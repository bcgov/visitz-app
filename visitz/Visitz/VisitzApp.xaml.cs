using Visitz.Authentication.Keycloak;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz;

public partial class VisitzApp : Application
{
    public ServiceHandler ServiceHandler { get; private set; }

    public event EventHandler<EventArgs> AppResumed;

    public VisitzApp()
    {
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

        await TryModalSecurityChecksAsync();
    }

    private static async Task TryModalSecurityChecksAsync()
    {
        await SessionPage.TryOpenAsync(modal: true, animated: false);

        if (await VisitzSession.SessionExistsAsync())
            await AppLockPage.TryPrompt();
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState activationState)
    {
        return SetWindowLayout(base.CreateWindow(activationState));
    }

    private static partial Window SetWindowLayout(Window window);
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
}
