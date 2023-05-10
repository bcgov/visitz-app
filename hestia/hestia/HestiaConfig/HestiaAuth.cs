using hestia.Services.Authentication;
using hestia.Settings;
using Microsoft.Extensions.Configuration;

namespace hestia.HestiaConfig
{
    public static class HestiaAuth
    {
        public static MauiAppBuilder ConfigureHestiaAuth(this MauiAppBuilder builder)
        {
            // TODO: Get AppSettings working correctly with DI
            var settings = new AppSettings().Oidc;

            var options = new AuthenticationClient.Options()
            {
                Domain = settings.AuthenticationDomain,
                ClientId = settings.ClientId,
                Scope = "",
                RedirectUri = settings.RedirectUri
            };

            builder.Services.AddSingleton(new AuthenticationClient(options));

            return builder;
        }
    }
}
