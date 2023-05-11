using visitz.Services.Networking;
using visitz.Settings;
using visitzApi;
using Microsoft.Extensions.Configuration;

namespace visitz.VisitzConfig
{
    public static class VisitzApiConfig
    {
        private const string HttpClientName = "VisitzApiClient";

        public static MauiAppBuilder ConfigureVisitzApi(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<TokenHandler>();

            builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<TokenHandler>();

            builder.Services.AddSingleton(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName));

            // TODO: Get AppSettings working correctly with DI
            var apiConfig = new AppSettings().Api.ApiDomain;

            builder.Services.AddSingleton(sp => 
                new VisitzApi(sp.GetService<HttpClient>(), apiConfig));

            return builder;
        }
    }
}
