using hestia.Models;
using hestia.Services.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hestia.HestiaConfig
{
    public class HestiaAuth
    {
        private static readonly string Namespace = "hestia";
        private static readonly string AppSettingsFile = "appSettings.json";

        private static readonly string OidcSettings = "OidcSettings";

        public static void ConfigureHestiaAuth(MauiAppBuilder builder)
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
        }
    }
}
