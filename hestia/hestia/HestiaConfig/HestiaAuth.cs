using hestia.Models;
using hestia.Services.Authentication;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace hestia.HestiaConfig
{
    public static class HestiaAuth
    {
        private static readonly string Namespace = "hestia";
        private static readonly string AppSettingsFile = "appSettings.json";

        private static readonly string OidcSettings = "OidcSettings";

        public static MauiAppBuilder ConfigureHestiaAuth(this MauiAppBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(Namespace + "." + AppSettingsFile);

            var config = new ConfigurationBuilder()
                         .AddJsonStream(stream)
                         .Build();
            builder.Configuration.AddConfiguration(config);

            var settings = config.GetRequiredSection(OidcSettings).Get<OidcSettings>();
            var authenticationOptions = new AuthenticationClientOptions()
            {
                Domain = settings.AuthenticationDomain,
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                Scope = "",
                RedirectUri = settings.RedirectUri
            };

            builder.Services.AddSingleton(new AuthenticationClient(authenticationOptions));

            return builder;
        }
    }
}
