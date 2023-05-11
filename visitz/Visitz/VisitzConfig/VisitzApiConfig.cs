using Visitz.Services.Networking;
using Visitz.Settings;
using VisitzApi;
using Microsoft.Extensions.Configuration;

namespace Visitz.VisitzConfig
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
                new Vpi(sp.GetService<HttpClient>(), apiConfig));

            return builder;
        }
    }
}
