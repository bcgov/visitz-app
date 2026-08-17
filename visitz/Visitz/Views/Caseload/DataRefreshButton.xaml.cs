using Oidc.Network;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

public partial class DataRefreshButton : ViewModelContentView<DataRefreshViewModel>
{
    public static readonly string RefreshGlyph = MaterialIcons.Refresh;

    public StackOrientation Orientation
    {
        get => ViewModel.Orientation;
        set => ViewModel.Orientation = value;
    }

    public DataRefreshButton()
        : base(ServiceProvider.GetService<DataRefreshViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        SetIconByNetworkAccess();
        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

        SetMenu();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
            disposed = true;
        }

        base.Dispose(disposing);
    }

    void SetMenu()
    {
        var item = new MenuFlyoutItem()
        {
            Text = LocalizedStrings.RefreshCaseload,
            Command = ViewModel.RefreshDataCommand,
        };
        item.KeyboardAccelerators.Add(new() { Key = "F5" });

        FlyoutBase.SetContextFlyout(this, new MenuFlyout() { item });
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        SetIconByNetworkAccess();
    }

    private void SetIconByNetworkAccess()
    {
        RefreshButton.Text = NetworkHelper.InternetAvailable ? RefreshGlyph : MaterialIcons.File_download_off;
    }
}
