using Visitz.Services.Authentication.Keycloak;
using Visitz.Settings;

namespace Visitz.VisitzConfig
{
    public static class VisitzAuth
    {
        public static MauiAppBuilder ConfigureVisitzAuth(this MauiAppBuilder builder)
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
