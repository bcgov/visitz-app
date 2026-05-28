namespace Oidc;

public static class OidcMauiAppBuilderExtension
{
    public static MauiAppBuilder ConfigureOidcSettings(this MauiAppBuilder builder, OidcSettings settings)
    {
        var options = new AuthenticationClient.Options()
        {
            Domain = settings.AuthenticationDomain,
            ClientId = settings.ClientId,
            RedirectUri = settings.RedirectUri,
        };

        builder.Services.AddSingleton(new AuthenticationClient(options));

        return builder;
    }
}
