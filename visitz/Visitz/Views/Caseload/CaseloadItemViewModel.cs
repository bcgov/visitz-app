using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Network;
using System.ComponentModel;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemViewModel : VisitzViewModel
{
    public static readonly FontImageSource RemoveImageSource = new()
    {
        FontFamily = FluentIcons.FontConfig.FontFamily,
        Glyph = FluentIcons.Subtract_circle_20_regular,
    };

    readonly ServiceHandler serviceHandler;

    OidcSessionInfo SessionInfo { get; }

    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public DraftIndicatorHelper indicatorHelper;

    [ObservableProperty]
    public bool showDate;

    [ObservableProperty]
    public bool showDraftIndicator;

    [ObservableProperty]
    public bool showDownloadIcon;

    [ObservableProperty]
    public bool showProgressIndicator;

    [ObservableProperty]
    public bool canRemoveFromDevice;

    public CaseloadItemViewModel(
        DraftIndicatorHelper indicatorHelper,
        IBusinessObject businessObject,
        OidcSessionInfo sessionInfo) : base()
    {
        IndicatorHelper = indicatorHelper;
        BusinessObject = businessObject;
        SessionInfo = sessionInfo;
        serviceHandler = ServiceProvider.GetService<ServiceHandler>();

        IndicatorHelper.PropertyChanged += IndicatorHelper_PropertyChanged;
        serviceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        serviceHandler.ServiceFinished += ServiceHandler_ServiceFinished;

        UpdateInteractiveStates();
        StartInitAsync();
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            IndicatorHelper.PropertyChanged -= IndicatorHelper_PropertyChanged;
            serviceHandler.ServiceStarted -= ServiceHandler_ServiceStarted;
            serviceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    protected override ILogger<VisitzViewModel> MakeLogger()
    {
        return ServiceProvider.GetService<ILogger<CaseloadItemViewModel>>();
    }

    void UpdateDraftIndicatorVisibility()
    {
        var draftItems = IndicatorHelper.DraftedItems;

        if (draftItems != null)
        {
            var idType = (BusinessObject.Id, BusinessObject.EntityType);
            var numType = (BusinessObject.FileNumber, BusinessObject.EntityType);

            ShowDraftIndicator = draftItems.Contains(idType) || draftItems.Contains(numType);
        }
        else
            ShowDraftIndicator = false;
    }

    void UpdateInteractiveStates()
    {
        UpdateStateVisibility();

        CanRemoveFromDevice = !BusinessObject.IsAssigned(SessionInfo.Idir)
            && BusinessObject.LocalState.ShouldDownloadDuringRefresh
            && !ServicesRunning();
    }

    void UpdateStateVisibility()
    {
        if (!BusinessObject.IsValid)
            return;

        var isRunning = serviceHandler.IsAnyServiceRunning(BusinessObject.Id);
        bool isntMarkedForDownload = !(BusinessObject.LocalState?.ShouldDownloadDuringRefresh ?? false);

        if (isRunning)
        {
            ShowProgressIndicator = true;
            ShowDownloadIcon = !ShowProgressIndicator;
        }
        else if (isntMarkedForDownload)
        {
            ShowProgressIndicator = false;
            ShowDownloadIcon = !ShowProgressIndicator;
        }
        else
        {
            ShowProgressIndicator = false;
            ShowDownloadIcon = false;
        }

        ShowDate = !ShowDownloadIcon && !ShowProgressIndicator;
    }

    void OpenEntityView()
    {
        var msg = new BusinessObjectSelectedMessage(BusinessObject);
        StrongReferenceMessenger.Default.Send(msg);
    }

    [RelayCommand]
    public async Task BusinessObjectSelected()
    {
        bool markForDownload = !BusinessObject.LocalState.ShouldDownloadDuringRefresh;

        if (markForDownload)
        {
            if (await BusinessObject.PromptCanDownloadDependentData())
            {
                BusinessObject.LocalState.ShouldDownloadDuringRefreshBinding = true;

                var msg = GetAllDataForRecordService.MakeStartMessage(BusinessObject);
                WeakReferenceMessenger.Default.Send(msg);
            }
            // else: cancel
        }
        else
            OpenEntityView();

        UpdateInteractiveStates();
    }

    [RelayCommand]
    public async Task UnloadDependentData()
    {
        if (ShowDownloadIcon)
        {
            await Navigator.CurrentOpenPage.DisplayAlert(
                LocalizedStrings.UnableToRemove,
                LocalizedStrings.RemoveFromDeviceErrorNotDownloaded,
                LocalizedStrings.Ok);
            return;
        }
        else if (BusinessObject.IsAssigned(SessionInfo.Idir))
        {
            string assignedMsg = string.Format(
                LocalizedStrings.RemoveFromDeviceErrorAssigned,
                BusinessObject.EntityType,
                BusinessObject.DisplayName.Trim());

            await Navigator.CurrentOpenPage.DisplayAlert(
                LocalizedStrings.UnableToRemove,
                assignedMsg,
                LocalizedStrings.Ok);
            return;
        }

        string message = string.Format(LocalizedStrings.RemoveFromDeviceMessage,
            BusinessObject.EntityType,
            BusinessObject.DisplayName);

        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.RemoveFromDevice,
            message,
            LocalizedStrings.RemoveFromDevice,
            LocalizedStrings.Cancel);

        if (shouldRemove && !ServicesRunning())
        {
            var ignoredPrefs = ServiceProvider.GetService<UserIgnoredContentPrefs>();

            await BusinessObject.CommitAsync(() =>
            {
                BusinessObject.LocalState.ShouldDownloadDuringRefresh = false;
                BusinessObject.DeleteDependentData(ignoredPrefs, deleteLocalState: false);
            });

            UpdateInteractiveStates();
        }
    }

    void IndicatorHelper_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(DraftIndicatorHelper.DraftedItems))
                UpdateDraftIndicatorVisibility();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ServiceHandler_ServiceStarted(object? sender, string e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateInteractiveStates);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ServiceHandler_ServiceFinished(object? sender, VisitzService service)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateInteractiveStates();

                if (service.GetId() == GetAllDataForRecordService.MakeId(BusinessObject)
                    && service.UncaughtException != null)
                {
                    _ = Navigator.CurrentOpenPage.DisplayErrorAlert(service.UncaughtException);
                }
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    bool ServicesRunning()
    {
        return serviceHandler.IsAnyServiceRunning(nameof(GetAllDataForOfflineService))
            || serviceHandler.IsAnyServiceRunning(BusinessObject.ToIdTypeString());
    }
}
