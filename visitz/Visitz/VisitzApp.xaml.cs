using Oidc;
using Oidc.Events;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;
using Visitz.Views.Root;
using VisitzModel;

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

	protected async override void OnStart()
    {
        ConsoleTrace.TraceMethod(this);

        base.OnStart();
        
        ServiceHandler = ServiceProvider.Current.GetService<ServiceHandler>();

        await TryModalSecurityChecksAsync();
    }

    protected async override void OnResume()
	{
        ConsoleTrace.TraceMethod(this);

        base.OnResume();
        AppResumed?.Invoke(this, null);

		if (ShouldTryModalSecurityChecksOnResume())
			await TryModalSecurityChecksAsync();
    }

	private static bool ShouldTryModalSecurityChecksOnResume()
	{
#if WINDOWS
		// Application.OnResume is invoked every time the window gains focus, including after a user
		// correctly enters their credentials in a modal dialog. To prevent an infinite loop of being
		// prompted to enter credentials on Windows, we'll only issue the auth challenge during
		// Application.OnStart.
		return false;
#else
		return true;
#endif
	}

    private static async Task TryModalSecurityChecksAsync()
    {
        await SessionPage.TryOpenAsync(modal: true, animated: false);

#if DEBUG
        if (DebugOptions.SkipLocalAuth)
            return;
#endif
        if (await OidcSession.SessionExistsAsync())
            await AppLockPage.TryPrompt();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
		var visitzWindow = new VisitzWindow(MainPage);

		visitzWindow.ActivatedWhenInvalid += VisitzWindow_ActivatedWhenInvalid;

		return visitzWindow;
    }

	private async void VisitzWindow_ActivatedWhenInvalid(object sender, EventArgs e)
	{
		await TryModalSecurityChecksAsync();
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
