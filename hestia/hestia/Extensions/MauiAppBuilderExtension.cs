using System;
using hestia.Services.Authentication;
using hestia.Services.Networking;
using hestia.Services;
using hestia.Views;
using hestia.ViewModels;
using hestia.Routers;
using System.Reflection;
using hestia.Services.Localization;
using Microsoft.Extensions.Configuration;
using hestia.Models;

namespace hestia.Extensions
{
    /// <summary>
    /// MauiAppBuilder's added functionality
    /// </summary>
    public static class MauiAppBuilderExtension
    {
        /// <summary>
        /// Dependency injection setup
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>MauiAppBuilder</returns>
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            // Reading environment variables
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("hestia.appSettings.json");
            var authenticationOptions = new AuthenticationClientOptions();
            // No support for ConfigurationBuilder -> AddJsonStream method in Android.
            // Compiltion condition can be removed once that is supported in any of the future releases.
#if !ANDROID
            var config = new ConfigurationBuilder()
                         .AddJsonStream(stream)
                         .Build();
            builder.Configuration.AddConfiguration(config);
            var settings = config.GetRequiredSection("AppSettings").Get<AppSettings>();
            authenticationOptions = new()
            {
                Domain = settings.AuthenticationDomain,
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                Scope = "",
                RedirectUri = settings.RedirectUri
            };
#endif
            // This service is needed to inject IStringLocalizer into LocalizeExtension
            builder.Services.AddLocalization();

            // IStringLocalizer appears to be dependent on a logging service 
            builder.Services.AddLogging();

            builder.Services.AddSingleton<LocalizeExtension>();

            builder.Services.AddSingleton<LandingPage>();
            builder.Services.AddSingleton<LandingRouter>();
            builder.Services.AddSingleton<LandingViewModel>();

            builder.Services.AddSingleton(new AuthenticationClient(authenticationOptions));

            builder.Services.AddSingleton<TokenHandler>();
            // Definition of a named HttpClient instance ("CasesAndIncidentsAPI")
            builder.Services.AddHttpClient("CasesAndIncidentsAPI",
                client => client.BaseAddress = new Uri("https://icmint620b-cysndevds.api.gov.bc.ca/")
                ).AddHttpMessageHandler<TokenHandler>();
            // Creation of the actual HttpClient instance
            builder.Services.AddTransient(
                sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CasesAndIncidentsAPI")
                );

            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<DeviceAuthenticationRouter>();
            builder.Services.AddTransient<DeviceAuthenticationPage>();
            builder.Services.AddTransient<DeviceAuthenticationViewModel>();

            builder.Services.AddTransient<OpenIdAuthenticationRouter>();
            builder.Services.AddTransient<OpenIdAuthenticationPage>();
            builder.Services.AddTransient<OpenIdAuthenticationViewModel>();

            builder.Services.AddTransient<CasesAndIncidentsPage>();
            builder.Services.AddTransient<CasesAndIncidentsViewModel>();

            return builder;
        }
    }
}

