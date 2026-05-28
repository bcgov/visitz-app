using System.Reflection;
using Microsoft.Extensions.Configuration;
using Oidc;

namespace Visitz.Settings;

public class AppSettings
{
    private static readonly string Namespace = "Visitz";
    private static readonly string AppSettingsFile = "appSettings.json";

    private static readonly string ApiSettingsKey = "ApiSettings";
    private static readonly string OidcSettingsKey = "OidcSettings";
    private static readonly string DebugSettingsKey = "DebugSettings";
    private static readonly string ContactInfoSettingsKey = "ContactInfoSettings";

    public static string AppSettingsPath => Namespace + "." + AppSettingsFile;

    public IConfiguration Configuration { get; }

    public ApiSettings Api => Configuration.GetRequiredSection(ApiSettingsKey).Get<ApiSettings>();

    public OidcSettings Oidc => Configuration.GetRequiredSection(OidcSettingsKey).Get<OidcSettings>();

    public DebugSettings Debug => Configuration.GetRequiredSection(DebugSettingsKey).Get<DebugSettings>();

    public ContactInfoSettings ContactInfo =>
        Configuration.GetRequiredSection(ContactInfoSettingsKey).Get<ContactInfoSettings>();

    public AppSettings()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(AppSettingsPath);

        Configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
    }
}
