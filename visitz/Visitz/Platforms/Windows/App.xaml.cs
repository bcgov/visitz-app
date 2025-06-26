// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppLifecycle;
using Oidc.WinWorkaround;
using Visitz.Platforms.Windows.Visitz;
using Visitz.Views.WebViewer;
using VisitzModel.Platforms.Windows.Logging;
using WebAuthenticator = Oidc.WinWorkaround.WebAuthenticator;

#nullable enable

namespace Visitz.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public CancellationTokenSource? AuthCancelTokenSource { get; set; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;

        if (HandleOAuthRedirect())
            return;

        try
        {
            HandleInstanceRedirect();
        }
        catch (Exception ex)
        {
            var redirectEx = new Exception("Failed redirect to existing app instance", ex);
            WriteExceptionToEventViewer(redirectEx);
        }
    }

    private bool HandleOAuthRedirect()
    {
        bool isOAuthRedirect = WebAuthenticator.CheckOAuthRedirectionActivation();

        if (!isOAuthRedirect)
            WebAuthenticator.PromptAuthentication += WebAuthenticator_PromptForCredentials;

        return isOAuthRedirect;
    }

    private void HandleInstanceRedirect()
    {
        var keyInstance = AppInstance.FindOrRegisterForKey(nameof(VisitzApp));

        if (!keyInstance.IsCurrent)
        {
            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            WindowUtil.RedirectActivationTo(args, keyInstance);
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        WriteExceptionToEventViewer(e.Exception);
    }

    private void WriteExceptionToEventViewer(Exception exception)
    {
        EventLogWriter.WriteEntry(
            LogLevel.Error,
            exception.Message,
            GetType().FullName,
            exception: exception);
    }

    private async void WebAuthenticator_PromptForCredentials(object? sender, InvokingAuthEventArgs e)
    {
        var webViewPage = ServiceProvider.GetService<WebViewPage>();

        webViewPage.AuthUri = e.Uri;
        webViewPage.CancelTokenSource = AuthCancelTokenSource;

        await Navigator.Navigation.PushModalAsync(webViewPage);
    }
}
