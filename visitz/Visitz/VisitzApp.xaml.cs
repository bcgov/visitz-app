using Visitz.Authentication.Keycloak;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz;

public partial class VisitzApp : Application
{
    public static INavigation Navigation => Current.MainPage.Navigation;

    public static Page CurrentOpenPage
    {
        get
        {
            int last = Navigation.NavigationStack.Count - 1;
            return last >= 0 ? Navigation.NavigationStack[last] : null;
        }
    }

    public static Page CurrentOpenModal
    {
        get
        {
            int last = Navigation.ModalStack.Count - 1;
            return last >= 0 ? Navigation.ModalStack[last] : null;
        }
    }

    public ServiceHandler ServiceHandler { get; private set; }

    public event EventHandler<EventArgs> AppResumed;

    public VisitzApp()
    {
        InitializeComponent();

        // TODO: Get this working with the DI system
        // DI setup has been disabled for now in VisitzScreens
        MainPage = new NavigationPage(CaseloadPage.GetInstance());

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
