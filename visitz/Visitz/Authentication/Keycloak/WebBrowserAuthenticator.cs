using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;
using Visitz.Resources.Localization;
using Visitz.Settings;

namespace Visitz.Authentication.Keycloak
{
    /// <summary>
    /// The class implements the IBrowser interface to handle the authentication step.
    /// In practice, this class is responsible for opening the system browser, which will show the user the Login page.
    /// </summary>
	public class WebBrowserAuthenticator : IdentityModel.OidcClient.Browser.IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                var callbackUrl = options.EndUrl?.Length > 0 
                    ? options.EndUrl 
                    : new AppSettings().Oidc.RedirectUri;

                WebAuthenticatorResult result = await WebAuthenticator.Default.AuthenticateAsync(new()
                {
                    Url = new Uri(options.StartUrl),
                    CallbackUrl = new Uri(callbackUrl),

                    /*
                        Right now, the best way to accomodate a "real" logout feature is to enable this option.
                        Keeping it disabled for now because, at worst, a user has to go through the login process 
                        every 30 minutes: enter work email, redirect to siteminder, enter gov email login, approve
                        login via authenticator (potentially a phone call), then they'd choose "remember me" even
                        though we would immediately dump the cookie because of this setting.

                        So while we should be using this setting, I think we shouldn't enable it until we implement
                        FIDO2 logins.
                     */
                    PrefersEphemeralWebBrowserSession = false,
                });

                var url = new RequestUrl(callbackUrl)
                    .Create(new Parameters(result.Properties));

                return new BrowserResult
                {
                    Response = url,
                    ResultType = BrowserResultType.Success
                };
            }
            catch (TaskCanceledException)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel,
                    ErrorDescription = LocalizedStrings.UserCancelledAuth
                };
            }
        }
    }
}

