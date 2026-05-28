using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Services.Messages;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Views.Caseload;

#nullable enable

public partial class CaseloadItemViewModel
    : VisitzViewModel,
        IComparable<CaseloadItemViewModel>,
        IRecipient<ServiceStateMessage>
{
    public static readonly FontImageSource RemoveImageSource = new()
    {
        FontFamily = FluentIcons.FontConfig.FontFamily,
        Glyph = FluentIcons.Subtract_circle_20_regular,
    };

    OidcSessionInfo SessionInfo { get; }

    readonly ServiceActivityListener _activityListener = new();

    [ObservableProperty]
    public partial IBusinessObject BusinessObject { get; set; }

    [ObservableProperty]
    public partial DraftIndicatorHelper IndicatorHelper { get; set; }

    [ObservableProperty]
    public partial bool ShowDraftIndicator { get; set; }

    [ObservableProperty]
    public partial bool ShowDownloadIcon { get; set; }

    [ObservableProperty]
    public partial bool ShowProgressIndicator { get; set; }

    [ObservableProperty]
    public partial bool CanRemoveFromDevice { get; set; }

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

        IndicatorHelper.PropertyChanged += IndicatorHelper_PropertyChanged;

        WeakReferenceMessenger.Default.Register(this, GetAllDataForRecordService.MakeId(BusinessObject));

        if (BusinessObject.LocalState?.ShouldDownloadDuringRefresh ?? false)
            RegisterForActivity();

        UpdateRecordStates();
        UpdateDraftIndicatorVisibility();
        StartInitAsync();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            UnregisterFromActivity();
            _activityListener.Dispose();

            WeakReferenceMessenger.Default.UnregisterAll(this);

            IndicatorHelper.PropertyChanged -= IndicatorHelper_PropertyChanged;

            BusinessObject = new CaseRecord();
            disposed = true;
        }
        base.Dispose(disposing);
    }

    void RegisterForActivity()
    {
        if (BusinessObject.IsValid && !_activityListener.IsRegistered)
        {
            _activityListener.RegisterForMessages(BusinessObject);
            _activityListener.Started += ActivityListener_ServicesStarted;
            _activityListener.Stopped += ActivityListener_ServicesStopped;
        }
    }

    void UnregisterFromActivity()
    {
        _activityListener.UnregisterFromMessages();
        _activityListener.Started -= ActivityListener_ServicesStarted;
        _activityListener.Stopped -= ActivityListener_ServicesStopped;
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
            && !_activityListener.HasActivity;
    }

    void UpdateStateVisibility()
    {
        if (!BusinessObject.IsValid)
            return;

        bool isntMarkedForDownload = !(BusinessObject.LocalState?.ShouldDownloadDuringRefresh ?? false);

        if (_activityListener.HasActivity)
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
                RegisterForActivity();

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

        if (shouldRemove && !_activityListener.HasActivity)
        {
            var ignoredPrefs = ServiceProvider.GetService<UserIgnoredContentPrefs>();

            await BusinessObject.CommitAsync(() =>
            {
                BusinessObject.LocalState?.ShouldDownloadDuringRefresh = false;
                BusinessObject.DeleteDependentData(ignoredPrefs, deleteLocalState: false);
            });
            UnregisterFromActivity();

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

    async void ActivityListener_ServicesStarted(object? sender, EventArgs empty)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateRecordStates);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    async void ActivityListener_ServicesStopped(object? sender, EventArgs empty)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateRecordStates);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    public int CompareTo(CaseloadItemViewModel? other)
    {
        return BusinessObject.IdBinding.CompareTo(other?.BusinessObject.IdBinding);
    }

    public async void Receive(ServiceStateMessage message)
    {
        try
        {
            if (message.FinishedError && message.UncaughtException != null)
            {
                Logger.LogError(message.UncaughtException);
                await Navigator.CurrentOpenPage.DisplayErrorAlert(message.UncaughtException);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }
}
