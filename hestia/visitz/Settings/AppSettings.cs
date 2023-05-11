using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Visitz.Settings
{
    public class AppSettings
    {
        private static readonly string Namespace = "Visitz";
        private static readonly string AppSettingsFile = "appSettings.json";

        private static readonly string ApiSettingsKey = "ApiSettings";
        private static readonly string OidcSettingsKey = "OidcSettings";

        public static string AppSettingsPath => Namespace + "." + AppSettingsFile;

        public IConfiguration Configuration { get; }

        public ApiSettings Api => Configuration.GetRequiredSection(ApiSettingsKey).Get<ApiSettings>();

        public OidcSettings Oidc => Configuration.GetRequiredSection(OidcSettingsKey).Get<OidcSettings>();

        public AppSettings()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(AppSettingsPath);

            Configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }
    }
}
