using Oidc;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;
using VisitzModel;

namespace Visitz;

public partial class VisitzApp : Application
{
    public ServiceHandler ServiceHandler { get; private set; }

    public event EventHandler<EventArgs> AppResumed;

    public VisitzApp()
    {
        InitializeComponent();

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
		var mainPage = new NavigationPage(ServiceProvider.GetService<RootPage>());
		var visitzWindow = new VisitzWindow(mainPage);

		visitzWindow.ActivatedWhenInvalid += VisitzWindow_ActivatedWhenInvalid;

		return visitzWindow;
    }

	private async void VisitzWindow_ActivatedWhenInvalid(object sender, EventArgs e)
	{
		await TryModalSecurityChecksAsync();
	}

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
}
