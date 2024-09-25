using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Visitz.Services;
using Visitz.Storage;
using Visitz.Views.Debugging;
using Visitz.Views.Root;

#if WINDOWS
using Visitz.Views.WebViewer;
#endif

namespace Visitz;

public partial class VisitzApp : Application
{
    public ServiceHandler ServiceHandler { get; private set; }

#if WINDOWS
    public CancellationTokenSource AuthCancelTokenSource { get; set; }
#endif

    private readonly ILogger<VisitzApp> _logger;
    public VisitzApp(ILogger<VisitzApp> logger)
    {
#if WINDOWS
        if (Oidc.WinWorkaround.WebAuthenticator.CheckOAuthRedirectionActivation())
            return;

        Oidc.WinWorkaround.WebAuthenticator.PromptAuthentication += WebAuthenticator_PromptForCredentials;
#endif
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
