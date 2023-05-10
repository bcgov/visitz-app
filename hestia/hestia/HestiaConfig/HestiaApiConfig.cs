using hestia.Services.Networking;

namespace hestia.HestiaConfig
{
    public static class HestiaApiConfig
    {
        private const string HttpClientName = "HestiApiClient";

        public static MauiAppBuilder ConfigureHestiaApi(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<TokenHandler>();

            builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<TokenHandler>();

            builder.Services.AddTransient(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName));

            return builder;
        }
    }
}
