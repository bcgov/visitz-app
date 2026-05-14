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

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityPageViewModel(ServiceHandler serviceHandler)
    : IcmRecordViewModel,
        IRecipient<ServiceStateMessage>
{
    bool _disposed;

    ServiceHandler ServiceHandler { get; } = serviceHandler;

    [ObservableProperty]
    public partial string DisplayName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FileNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool DownloadActivity { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        BusinessObject.SubscribePropertyChanged(BusinessObject_PropertyChanged);
        UpdateDownloadActivity();

        WeakReferenceMessenger.Default.Register(this, GetAllDataForRecordService.MakeId(BusinessObject));

        ServiceHandler.ServiceStarted += ServiceHandler_ServiceStarted;
        ServiceHandler.ServiceFinished += ServiceHandler_ServiceFinished;

        UpdateLocalActivityTimestamp();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ServiceHandler.ServiceFinished -= ServiceHandler_ServiceFinished;
            ServiceHandler.ServiceStarted -= ServiceHandler_ServiceStarted;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            BusinessObject.UnsubscribePropertyChanged(BusinessObject_PropertyChanged);

            _disposed = true;
        }
        base.Dispose(disposing);
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
        DownloadActivity = BusinessObject.IsValid && ServiceHandler.IsAnyServiceRunning(BusinessObject.Id);
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.FinishedError)
        {
            string displayString = $"{EntityType.GetDisplayString()} {DisplayName}";
            string msg = string.Format(LocalizedStrings.DownloadRecordErrorMessage, displayString);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(
                msg,
                message.UncaughtException?.ToString() ?? string.Empty,
                LocalizedStrings.DownloadError
            );
        }
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
            string.Format(LocalizedStrings.RecordRemovedFromCaseload, typeString, DisplayName),
            string.Format(LocalizedStrings.RecordRemovedFromCaseloadDetails, typeString, DisplayName),
            LocalizedStrings.Ok
        );
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }

    void UpdateLocalActivityTimestamp()
    {
        if (BusinessObject.IsValid)
            BusinessObject.LocalState?.LastOpenedBinding = DateTimeOffset.UtcNow;
    }
}
