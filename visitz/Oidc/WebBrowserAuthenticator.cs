using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;

namespace Oidc
{
    /// <summary>
    /// The class implements the IBrowser interface to handle the authentication step.
    /// In practice, this class is responsible for opening the system browser, which will show the user the Login page.
    /// </summary>
	public class WebBrowserAuthenticator : IdentityModel.OidcClient.Browser.IBrowser
    {
        private const string EncodedHashtag = "%23";

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                var callbackUrl = options.EndUrl?.Length > 0
                    ? options.EndUrl
                    : redirect URI here;

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
                    Response = FixEncodedResponseUrl(url),
                    ResultType = BrowserResultType.Success
                };
            }
            catch (TaskCanceledException)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel,
                    ErrorDescription = BrowserResultType.UserCancel.ToString()
                };
            }
            catch (Exception ex)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UnknownError,
                    ErrorDescription = ex.Message
                };
            }
        }

        /*
         * BUG: There seems to be an issue when using Azure OIDC and .NET MAUI's WebAuthenticator. The 
         * WebAuthenticationResult returned from AuthenticateAsync() appends a hashtag ('#') to the
         * callback URL for [some reason]. In my testing the authorization code was always the last
         * parameter in the string, so when going to exchange the auth code for an access token it would
         * be rejected with an "invalid_grant" error (because of course it would. Why would an authorization 
         * code have a hashtag arbitrarily appended to it?).
         * 
         * If we switch to use a different IDP that doesn't append characters for [reasons] or .NET MAUI 
         * fixes the issue in the platform we could then remove this function.
         * 
         * - Todd S.
         */
        private static string FixEncodedResponseUrl(string url)
        {
            if (url.EndsWith(EncodedHashtag))
                url = url[..url.LastIndexOf(EncodedHashtag)];

            return url;
        }
    }
}
