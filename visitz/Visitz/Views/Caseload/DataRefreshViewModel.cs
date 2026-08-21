using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Services.Messages;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

public partial class DataRefreshViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
{
    [ObservableProperty]
    public partial bool CanShowSuperMessage { get; set; } = true;

    [ObservableProperty]
    public partial bool SuperMessageVisible { get; set; } = false;

    [ObservableProperty]
    public partial string SuperMessage { get; set; }

    [ObservableProperty]
    public partial StackOrientation Orientation { get; set; } = StackOrientation.Vertical;

    [ObservableProperty]
    public partial bool CaseloadActivity { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        SetConnectivityMessage();
        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        SetConnectivityMessage();
    }

    private void SetConnectivityMessage()
    {
        SuperMessage = NetworkHelper.InternetAvailable ? "" : LocalizedStrings.Offline;
    }

    [RelayCommand]
    public static void RefreshData()
    {
        WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage(forceDownload: true));
    }

    partial void OnSuperMessageChanged(string value)
    {
        ApplyVisibility();
    }

    partial void OnCanShowSuperMessageChanged(bool value)
    {
        ApplyVisibility();
    }

    void ApplyVisibility()
    {
        SuperMessageVisible = CanShowSuperMessage && !string.IsNullOrWhiteSpace(SuperMessage);
    }

    public void Receive(ServiceStateMessage message)
    {
        CaseloadActivity = message.Status == VisitzService.State.Running;
    }

    [RelayCommand]
    public void HideIndicator()
    {
        CaseloadActivity = false;
    }
}
