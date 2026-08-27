using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;

namespace Oidc;

/// <summary>
/// The class exposes the Browser property and provides the LoginAsync() method to start the authentication process.
/// </summary>
public class AuthenticationClient
{
    private readonly OidcClient oidcClient;

    public AuthenticationClient(Options options)
    {
        oidcClient = new OidcClient(
            new OidcClientOptions
            {
                Authority = options.Domain,
                ClientId = options.ClientId,
                Scope = options.Scope,
                RedirectUri = options.RedirectUri,
                Browser = options.Browser,
                PostLogoutRedirectUri = options.RedirectUri,
                DisablePushedAuthorization = true,
            }
        );
    }

    public IdentityModel.OidcClient.Browser.IBrowser Browser
    {
        get { return oidcClient.Options.Browser; }
        set { oidcClient.Options.Browser = value; }
    }

    public async Task<LoginResult> LoginAsync(CancellationToken cancellationToken = default)
    {
        return await oidcClient.LoginAsync(cancellationToken: cancellationToken);
    }

    public async Task<LogoutResult> LogoutAsync()
    {
        return await oidcClient.LogoutAsync(
            new LogoutRequest()
            {
                BrowserDisplayMode = DisplayMode.Hidden,
                IdTokenHint = await TokenHolder.GetIdentityTokenStringAsync(),
            }
        );
    }

    public async Task<RefreshTokenResult> RefreshAsync(string refreshToken)
    {
        return await oidcClient.RefreshTokenAsync(refreshToken);
    }

    public class Options
    {
        private static readonly string DefaultScope = "openid email profile azureidir";

        public string Domain { get; set; }

        public string ClientId { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public IdentityModel.OidcClient.Browser.IBrowser Browser { get; set; }

        public Options()
        {
            Scope = DefaultScope;
            RedirectUri = "";
            Browser = new WebBrowserAuthenticator();
        }
    }
}
