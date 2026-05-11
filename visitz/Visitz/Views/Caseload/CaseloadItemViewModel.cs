using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
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
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemViewModel : VisitzViewModel, IComparable<CaseloadItemViewModel>
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
    public bool showDraftIndicator;

    [ObservableProperty]
    public bool showDownloadIcon;

    [ObservableProperty]
    public bool showProgressIndicator;

    [ObservableProperty]
    public bool canRemoveFromDevice;

    protected override ILogger<VisitzViewModel> Logger { get; } =
        ServiceProvider.GetService<ILogger<CaseloadItemViewModel>>();

    public CaseloadItemViewModel(
        DraftIndicatorHelper indicatorHelper,
        IBusinessObject businessObject,
        OidcSessionInfo sessionInfo
    )
        : base()
    {
        IndicatorHelper = indicatorHelper;
        BusinessObject = businessObject;
        SessionInfo = sessionInfo;
        serviceHandler = ServiceProvider.GetService<ServiceHandler>();

        IndicatorHelper.PropertyChanged += IndicatorHelper_PropertyChanged;
        serviceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        serviceHandler.ServiceFinished += ServiceHandler_ServiceFinished;

        UpdateRecordStates();
        UpdateDraftIndicatorVisibility();
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

            BusinessObject = new CaseRecord();
            disposed = true;
        }
        base.Dispose(disposing);
    }

    void UpdateDraftIndicatorVisibility()
    {
        if (
            BusinessObject.IsValid
            && IndicatorHelper.DraftedItems is HashSet<(string EntityId, EntityType Type)> draftItems
        )
        {
            var idType = (BusinessObject.Id, BusinessObject.EntityType);
            var numType = (BusinessObject.FileNumber, BusinessObject.EntityType);

            ShowDraftIndicator = draftItems.Contains(idType) || draftItems.Contains(numType);
        }
        else
            ShowDraftIndicator = false;
    }

    void UpdateRecordStates()
    {
        UpdateStateVisibility();

        CanRemoveFromDevice =
            BusinessObject.IsValid
            && !BusinessObject.IsAssigned(SessionInfo.Idir)
            && (BusinessObject.LocalState?.ShouldDownloadDuringRefresh ?? false)
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
    }

    void OpenEntityView()
    {
        var msg = new BusinessObjectSelectedMessage(BusinessObject);
        StrongReferenceMessenger.Default.Send(msg);
    }

    [RelayCommand]
    public async Task BusinessObjectSelected()
    {
        bool shouldDownload = BusinessObject.LocalState?.ShouldDownloadDuringRefresh ?? true;
        bool markForDownload = !shouldDownload;

        if (markForDownload)
        {
            if (await BusinessObject.PromptCanDownloadDependentData())
            {
                BusinessObject.LocalState?.ShouldDownloadDuringRefreshBinding = true;

                var msg = GetAllDataForRecordService.MakeStartMessage(BusinessObject);
                WeakReferenceMessenger.Default.Send(msg);
            }
            // else: cancel
        }
        else
            OpenEntityView();

        UpdateRecordStates();
    }

    [RelayCommand]
    public async Task UnloadDependentData()
    {
        if (ShowDownloadIcon)
        {
            await Navigator.CurrentOpenPage.DisplayAlertAsync(
                LocalizedStrings.UnableToRemove,
                LocalizedStrings.RemoveFromDeviceErrorNotDownloaded,
                LocalizedStrings.Ok
            );
            return;
        }
        else if (BusinessObject.IsAssigned(SessionInfo.Idir))
        {
            string assignedMsg = string.Format(
                LocalizedStrings.RemoveFromDeviceErrorAssigned,
                BusinessObject.EntityType,
                BusinessObject.DisplayName.Trim()
            );

            await Navigator.CurrentOpenPage.DisplayAlertAsync(
                LocalizedStrings.UnableToRemove,
                assignedMsg,
                LocalizedStrings.Ok
            );
            return;
        }

        string message = string.Format(
            LocalizedStrings.RemoveFromDeviceMessage,
            BusinessObject.EntityType,
            BusinessObject.DisplayName
        );

        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.RemoveFromDevice,
            message,
            LocalizedStrings.RemoveFromDevice,
            LocalizedStrings.Cancel
        );

        if (shouldRemove && !ServicesRunning())
        {
            var ignoredPrefs = ServiceProvider.GetService<UserIgnoredContentPrefs>();

            await BusinessObject.CommitAsync(() =>
            {
                BusinessObject.LocalState?.ShouldDownloadDuringRefresh = false;
                BusinessObject.DeleteDependentData(ignoredPrefs, deleteLocalState: false);
            });

            UpdateRecordStates();
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
            MainThread.BeginInvokeOnMainThread(UpdateRecordStates);
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
                UpdateRecordStates();

                if (
                    BusinessObject.IsValid
                    && service.GetId() == GetAllDataForRecordService.MakeId(BusinessObject)
                    && service.UncaughtException != null
                )
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

    public int CompareTo(CaseloadItemViewModel? other)
    {
        return BusinessObject.IdBinding.CompareTo(other?.BusinessObject.IdBinding);
    }
}
