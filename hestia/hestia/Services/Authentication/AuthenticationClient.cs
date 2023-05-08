using System;
using IdentityModel.OidcClient;

namespace hestia.Services.Authentication
{
    /// <summary>
    /// The class exposes the Browser property and provides the LoginAsync() method to start the authentication process.
    /// </summary>
	public class AuthenticationClient
    {
        private readonly OidcClient oidcClient;

        public AuthenticationClient(AuthenticationClientOptions options)
        {
            oidcClient = new OidcClient(new OidcClientOptions
            {
                Authority = options.Domain,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
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
    }
}

