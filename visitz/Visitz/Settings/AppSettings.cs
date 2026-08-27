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

    public ApiSettings Api => GetOrThrow<ApiSettings>(ApiSettingsKey);

    public OidcSettings Oidc => GetOrThrow<OidcSettings>(OidcSettingsKey);

    public DebugSettings Debug => GetOrThrow<DebugSettings>(DebugSettingsKey);

    public ContactInfoSettings ContactInfo => GetOrThrow<ContactInfoSettings>(ContactInfoSettingsKey);

    public AppSettings()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream =
            assembly.GetManifestResourceStream(AppSettingsPath)
            ?? throw new InvalidOperationException("App settings missing");

        Configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
    }

    TSettings GetOrThrow<TSettings>(string key)
    {
        return Configuration.GetRequiredSection(key).Get<TSettings>()
            ?? throw new InvalidOperationException(
                typeof(TSettings).Name + $"'s configuration is missing (using key '{key}')"
            );
    }
}
