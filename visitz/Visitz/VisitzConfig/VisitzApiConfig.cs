using Oidc;
using Visitz.Settings;
using VisitzApi;
using VisitzApi.Network;

namespace Visitz.VisitzConfig;

public static class VisitzApiConfig
{
    private const string HttpClientName = "VisitzApiClient";

    public static MauiAppBuilder ConfigureVisitzApi(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<AppendTokenHandler>();
        builder.Services.AddSingleton<AppendAcceptLanguageHandler>();
        builder.Services.AddSingleton<ClientSideRateLimitedHandler>();

        builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<AppendTokenHandler>();
        builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<AppendAcceptLanguageHandler>();
        builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<ClientSideRateLimitedHandler>();

        builder.Services.AddSingleton(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName)
        );

        // TODO: Get AppSettings working correctly with DI
        var apiConfig = new AppSettings().Api.ApiDomain;

        builder.Services.AddSingleton(sp => new Vpi(sp.GetService<HttpClient>(), apiConfig));

        return builder;
    }
}
