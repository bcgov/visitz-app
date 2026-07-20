using CommunityToolkit.Maui;
using Oidc.Network;
using Visitz.FontIcons;
using Visitz.Resources.Localization;

namespace Visitz.Views;

#nullable enable

public partial class InternetInfoView : ContentView, IDisposable
{
    bool _disposedValue;

    [BindableProperty]
    public partial bool ShouldShowView { get; set; }

    [BindableProperty]
    public partial string Message { get; set; }

    [BindableProperty]
    public partial ImageSource ImageSource { get; set; }

    [BindableProperty]
    public partial Color Color { get; set; }

    [BindableProperty]
    public partial bool ShowText { get; set; } = true;

    public InternetInfoView()
    {
        InitializeComponent();

        Message = LocalizedStrings.NoInternet;
        Color = Colors.Red;
        ImageSource = MaterialIcons.Signal_disconnected.GetUnfilledMaterialIcon(Color);

        ApplyConnectivityStyles();

        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        ApplyConnectivityStyles();
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        ApplyConnectivityStyles();
    }

    private void ApplyConnectivityStyles()
    {
        ShouldShowView = !NetworkHelper.InternetAvailable;
    }
}
