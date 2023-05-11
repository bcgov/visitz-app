using visitz.Resources.Localization;
using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;

namespace visitz.Services.Authentication
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
                WebAuthenticatorResult result = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri(options.StartUrl),
                    new Uri(options.EndUrl));

                var url = new RequestUrl(options.EndUrl)
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

