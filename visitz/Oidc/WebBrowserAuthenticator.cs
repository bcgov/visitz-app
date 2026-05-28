using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;

namespace Oidc;

/// <summary>
/// The class implements the IBrowser interface to handle the authentication step.
/// In practice, this class is responsible for opening the system browser, which will show the user the Login page.
/// </summary>
public class WebBrowserAuthenticator : IdentityModel.OidcClient.Browser.IBrowser
{
    private const string EncodedHashtag = "%23";

    public async Task<BrowserResult> InvokeAsync(
        BrowserOptions options,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
#if WINDOWS
            var result = await WinWorkaround.WebAuthenticator.AuthenticateAsync(
                new Uri(options.StartUrl),
                new Uri(options.EndUrl),
                cancellationToken
            );
#else
            WebAuthenticatorResult result = await WebAuthenticator.Default.AuthenticateAsync(
                new()
                {
                    Url = new Uri(options.StartUrl),
                    CallbackUrl = new Uri(options.EndUrl),

                    /*
                        Right now, the best way to accomodate a "real" logout feature is to enable this option.
                        With it disabled, the user's login will be stored in their cookies--so if they "log out"
                        in the app but don't follow the IDP's logout, their credentials will remain in cookies
                        and they'll be auto-logged in. Which could lead to confusion.

                        Keeping it disabled for now because, if enabled, a user has to go through the login process
                        every 30 minutes: enter credentials, approve login via authenticator (potentially a phone call),
                        then they'd choose "remember me" even though we would immediately dump the cookie.

                        So while we should be using this setting, I think we shouldn't enable it until we implement
                        FIDO2 logins.
                     */
                    PrefersEphemeralWebBrowserSession = false,
                }
            );
#endif
            var url = new RequestUrl(options.EndUrl).Create(new Parameters(result.Properties));

            return new BrowserResult
            {
                Response = FixEncodedResponseUrl(url),
                ResultType = BrowserResultType.Success,
            };
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException || ex is OperationCanceledException)
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel,
                    ErrorDescription = BrowserResultType.UserCancel.ToString(),
                };
            else
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UnknownError,
                    ErrorDescription = ex.Message,
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
