using Microsoft.Maui.Graphics.Text;
using Microsoft.Maui.Networking;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;

namespace Visitz.Views;

public partial class InternetInfoView : ContentView
{
    public static readonly BindableProperty ShouldShowViewProperty =
        BindableProperty.Create(nameof(ShouldShowView), typeof(bool), typeof(InternetInfoView));

    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(InternetInfoView));

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(InternetInfoView));

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(TagView));

    public bool ShouldShowView
    {
        get => (bool)GetValue(ShouldShowViewProperty);
        set => SetValue(ShouldShowViewProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public InternetInfoView()
	{
		InitializeComponent();
        BindingContext = this;
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        ApplyConnectivityStyles(Connectivity.Current.NetworkAccess);
    }

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        if (args.AttachingToParent())
            Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

        else if (args.DetachingFromParent())
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
    }

    private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        ApplyConnectivityStyles(e.NetworkAccess);
    }

    private void ApplyConnectivityStyles(NetworkAccess networkAccess)
    {
        ShouldShowView = networkAccess != NetworkAccess.Internet;

        Message = LocalizedStrings.NoInternet;
        Color = Colors.Red;
        ImageSource = MaterialIcons.Signal_disconnected.GetUnfilledMaterialIcon(Color);
    }

    // REVIEW: We *could* show a more meaningful message when connected without internet, but it turns out
    // that trying to use yellow on a whitebackground is an awful undertaking.
    private void LocalNetworkStyles(NetworkAccess networkAccess)
    {
        switch (networkAccess)
        {
            case NetworkAccess.Unknown:
            case NetworkAccess.ConstrainedInternet:
            case NetworkAccess.None:
                Message = LocalizedStrings.NoInternet;
                Color = Colors.Red;
                ImageSource = MaterialIcons.Signal_disconnected.GetUnfilledMaterialIcon(Color);
                break;
            case NetworkAccess.Local:
                Message = LocalizedStrings.ConnectedNoInternet;
                Color = Color.FromArgb("#ccb800");
                ImageSource = FluentIcons.Wifi_warning_20_regular.GetFluentIcon(Color);
                break;
        }
    }
}