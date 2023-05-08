using hestia.Services.Networking;

namespace hestia.HestiaConfig
{
    public class HestiaApiConfig
    {
        private const string HttpClientName = "CasesAndIncidentsAPI";

        public static MauiAppBuilder ConfigureApi(MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<TokenHandler>();

            // Definition of a named HttpClient instance ("CasesAndIncidentsAPI")
            builder.Services.AddHttpClient(HttpClientName).AddHttpMessageHandler<TokenHandler>();

            // Creation of the actual HttpClient instance
            builder.Services.AddTransient(sp => 
                sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName));

            return builder;
        }
    }
}
