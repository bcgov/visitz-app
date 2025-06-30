using Microsoft.Extensions.Logging;
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

    private readonly ILogger<VisitzApp> _logger;
    public VisitzApp(ILogger<VisitzApp> logger)
    {
        _logger = logger;

        OidcSession.SessionChanged += OidcSession_SessionChanged;

        InitializeComponent();

        MainPage = new NavigationPage(new RootPage());

        TryStartDebugSensor();
    }

    protected override void OnStart()
    {
        base.OnStart();

        ServiceHandler = ServiceProvider.Current.GetService<ServiceHandler>();
        try
        {
            _ = ClearRealmLogs.ClearLogData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new VisitzWindow(MainPage);
    }

    private static void TryStartDebugSensor()
    {
        DebugOptions.TryStartShakeDetector(actionOnShake: async () => await DebugOptionsPage.TryOpen());
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
