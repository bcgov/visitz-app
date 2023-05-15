using IdentityModel.OidcClient;

namespace Visitz.Services.Authentication
{
    /// <summary>
    /// The class exposes the Browser property and provides the LoginAsync() method to start the authentication process.
    /// </summary>
	public class AuthenticationClient
    {
        private readonly OidcClient oidcClient;

        public AuthenticationClient(Options options)
        {
            oidcClient = new OidcClient(new OidcClientOptions
            {
                Authority = options.Domain,
                ClientId = options.ClientId,
                Scope = options.Scope,
                RedirectUri = options.RedirectUri,
                Browser = options.Browser,
            });
        }

        public IdentityModel.OidcClient.Browser.IBrowser Browser
        {
            get
            {
                return oidcClient.Options.Browser;
            }
            set
            {
                oidcClient.Options.Browser = value;
            }
        }

        public async Task<LoginResult> LoginAsync()
        {
            return await oidcClient.LoginAsync();
        }

        public struct Options
        {
            public string Domain { get; set; }

            public string ClientId { get; set; }

            public string RedirectUri { get; set; }

            public string Scope { get; set; }

            public IdentityModel.OidcClient.Browser.IBrowser Browser { get; set; }

            public Options()
            {
                Scope = "";
                RedirectUri = "";
                Browser = new WebBrowserAuthenticator();
            }
        }
    }
}

