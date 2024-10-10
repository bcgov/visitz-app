using Oidc;
using Oidc.Events;
using Visitz.Services;
using Visitz.Storage;
using Visitz.Views.Debugging;
using Visitz.Views.Root;

namespace Visitz;

public partial class VisitzApp : Application
{
    public ServiceHandler ServiceHandler { get; private set; }

    public event EventHandler<EventArgs> AppResumed;

#if WINDOWS
	public CancellationTokenSource AuthCancelTokenSource { get; set; }
#endif

	public VisitzApp()
    {
#if WINDOWS
		if (Oidc.WinWorkaround.WebAuthenticator.CheckOAuthRedirectionActivation())
			return;

		Oidc.WinWorkaround.WebAuthenticator.PromptAuthentication += WebAuthenticator_PromptForCredentials;
#endif

		OidcSession.SessionChanged += OidcSession_SessionChanged;

        InitializeComponent();

        MainPage = new NavigationPage(new RootPage());

        TryStartDebugSensor();
    }

	protected override void OnStart()
    {
        base.OnStart();
        
        ServiceHandler = ServiceProvider.Current.GetService<ServiceHandler>();
    }

    protected override void OnResume()
	{
        base.OnResume();
        AppResumed?.Invoke(this, null);
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
		return new VisitzWindow(MainPage);
    }

#if WINDOWS
	private async void WebAuthenticator_PromptForCredentials(object sender, Oidc.WinWorkaround.InvokingAuthEventArgs e)
	{
		var webViewPage = ServiceProvider.GetService<WebViewPage>();

		webViewPage.AuthUri = e.Uri;
		webViewPage.CancelTokenSource = AuthCancelTokenSource;

		await Navigator.Navigation.PushModalAsync(webViewPage);
	}
#endif

	private static void TryStartDebugSensor()
    {
        if (DebugOptions.Enabled)
            TryStartShakeDetector();
    }

    private static void TryStartShakeDetector()
    {
        if (Accelerometer.Default.IsSupported)
        {
            Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
            Accelerometer.Default.Start(SensorSpeed.Game);
        }
        else
            Console.WriteLine("Accelerometer not supported");
    }

    private static async void Accelerometer_ShakeDetected(object sender, EventArgs e)
    {
        await DebugOptionsPage.TryOpen();
    }

	private void OidcSession_SessionChanged(object sender, SessionChangedEventArgs e)
	{
		if (e is LogoutChangedEventArgs args && args.Success)
			_ = ClearIcmData();
	}

	private static async Task ClearIcmData()
	{
		await (await VisitzRealms.GetIcmDataAsync()).ClearAllData();
	}
}
