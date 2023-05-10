using hestia.Services.Networking;
using hestia.Settings;
using hestiapi;
using Microsoft.Extensions.Configuration;

namespace hestia.HestiaConfig
{
    public static class HestiaApiConfig
    {
        private const string HttpClientName = "HestiApiClient";

        public static MauiAppBuilder ConfigureHestiaApi(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<TokenHandler>();

            builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<TokenHandler>();

            builder.Services.AddSingleton(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName));

            // TODO: Get AppSettings working correctly with DI
            var apiConfig = new AppSettings().Api.ApiDomain;

            builder.Services.AddSingleton(sp => 
                new HestiApi(sp.GetService<HttpClient>(), apiConfig));

            return builder;
        }
    }
}
