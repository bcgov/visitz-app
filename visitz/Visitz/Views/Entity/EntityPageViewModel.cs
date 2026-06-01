using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Services.Messages;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityPageViewModel(ServiceActivityListener activityListener)
    : IcmRecordViewModel,
        IRecipient<ServiceStateMessage>
{
    bool _disposed;

    bool _showId = true;

    readonly ServiceActivityListener _activityListener = activityListener;

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

        WeakReferenceMessenger.Default.Register(this, GetAllDataForRecordService.MakeId(BusinessObject));

        _activityListener.Started += ActivityListener_Started;
        _activityListener.Stopped += ActivityListener_Stopped;
        _activityListener.RegisterForMessages(BusinessObject);
        DownloadActivity = _activityListener.HasActivity;

        UpdateLocalActivityTimestamp();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _activityListener.Started -= ActivityListener_Started;
            _activityListener.Stopped -= ActivityListener_Stopped;
            _activityListener.Dispose();

            WeakReferenceMessenger.Default.UnregisterAll(this);

            BusinessObject.UnsubscribePropertyChanged(BusinessObject_PropertyChanged);

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void ActivityListener_Started(object? sender, EventArgs empty)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() => DownloadActivity = true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    void ActivityListener_Stopped(object? sender, EventArgs empty)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() => DownloadActivity = false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
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

    [RelayCommand]
    public void SwitchNumberAndId()
    {
        _showId = !_showId;
        FileNumber = _showId ? BusinessObject.FileNumber : BusinessObject.Id;
    }
}
