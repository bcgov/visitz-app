using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityViewModel : IcmRecordViewModel, IRecipient<ServiceStateMessage>
{
    ServiceHandler ServiceHandler { get; } = ServiceProvider.GetService<ServiceHandler>();

    string? _cacheRemovedDisplayName;

    public EntitySection? RequestedSection { get; set; }

    public IDraftItem? FocusedDraftItem { get; set; }

    [ObservableProperty]
    public bool downloadActivity;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (BusinessObject == null)
            return;

        _cacheRemovedDisplayName = BusinessObject.DisplayName;
        BusinessObject.SubscribePropertyChanged(BusinessObject_PropertyChanged);
        WeakReferenceMessenger.Default.Register(this, GetAllDataForRecordService.MakeId(BusinessObject));

        try
        {
            BuildNavList();

            if (RequestedSection != null)
                SelectedTab = GetMappedNavItem(RequestedSection);

            UpdateDownloadActivity();

            ServiceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
            ServiceHandler.ServiceFinished += ServiceHandler_ServiceFinished;

            UpdateLocalActivityTimestamp();
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            DisposeTabViews();

            BusinessObject?.UnsubscribePropertyChanged(BusinessObject_PropertyChanged);

            ServiceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;
            ServiceHandler.ServiceStarted -= ServiceHandler_ServiceStarted;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }
        base.Dispose(disposing);
    }

    async void BusinessObject_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IBusinessObject bobj)
            return;

        if (e.PropertyName == nameof(bobj.IsValid) && !bobj.IsValid)
            await EntityUnassignedGoBack();
    }

    async Task EntityUnassignedGoBack()
    {
        GoBack();

        string typeString = EntityType.GetDisplayString();

        await Navigator.CurrentOpenPage.DisplayAlertAsync(
            string.Format(LocalizedStrings.RecordRemovedFromCaseload, typeString, _cacheRemovedDisplayName),
            string.Format(LocalizedStrings.RecordRemovedFromCaseloadDetails, typeString, _cacheRemovedDisplayName),
            LocalizedStrings.Ok
        );
    }

    void ServiceHandler_ServiceStarted(object? sender, string e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateDownloadActivity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ServiceHandler_ServiceFinished(object? sender, VisitzService e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(UpdateDownloadActivity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void UpdateDownloadActivity()
    {
        if (BusinessObject != null)
            DownloadActivity = BusinessObject.IsValid && ServiceHandler.IsAnyServiceRunning(BusinessObject.Id);
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.FinishedError)
        {
            string displayString = $"{EntityType.GetDisplayString()} {_cacheRemovedDisplayName}";
            string msg = string.Format(LocalizedStrings.DownloadRecordErrorMessage, displayString);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(
                msg,
                message.UncaughtException?.ToString(),
                LocalizedStrings.DownloadError
            );
        }
    }

    void UpdateLocalActivityTimestamp()
    {
        if (BusinessObject != null && BusinessObject.IsValid)
            BusinessObject.LocalState.LastOpenedBinding = DateTimeOffset.UtcNow;
    }
}
