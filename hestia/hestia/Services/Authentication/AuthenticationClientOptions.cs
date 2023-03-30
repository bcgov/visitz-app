using System;
namespace hestia.Services.Authentication
{
    /// <summary>
    /// The class defined here collects the configuration settings for Authentication
    /// </summary>
	public class AuthenticationClientOptions
    {
        public AuthenticationClientOptions()
        {
            Scope = "";
            RedirectUri = "hestia://client";
            Browser = new WebBrowserAuthenticator();
        }

        public string Domain { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public IdentityModel.OidcClient.Browser.IBrowser Browser { get; set; }
    }
}

