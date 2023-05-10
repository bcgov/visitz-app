using hestia.Services.Networking;
using hestiapi;

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

            builder.Services.AddSingleton(sp =>
                new HestiApi(sp.GetService<HttpClient>(), "https://hestia-dev.api.gov.bc.ca"));

            return builder;
        }
    }
}
