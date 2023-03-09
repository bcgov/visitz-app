using System;
using Microsoft.Extensions.Logging;
using hestia.Services.Authentication;
using hestia.Services;
using hestia.Views;
using hestia.ViewModels;
using hestia.Routers;
using static System.Formats.Asn1.AsnWriter;

namespace hestia
{
    /// <summary>
    /// Application setup and configurations. (Separation of Concerns)
    /// </summary>
	public class HestiaApp
	{
        public static MauiApp Create()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Dependency Injection
            builder.Services.AddSingleton<LandingPage>();
            builder.Services.AddSingleton<LandingRouter>();
            builder.Services.AddSingleton<LandingViewModel>();

            builder.Services.AddSingleton(new AuthenticationClient(new()
            {
                Domain = "dev.loginproxy.gov.bc.ca/auth/realms/standard",
                ClientId = "mcfd-mobility-4577",
                Scope = "",
                RedirectUri = "hestia://client"
            }));

            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<DeviceAuthenticationRouter>();
            builder.Services.AddTransient<DeviceAuthenticationPage>();
            builder.Services.AddTransient<DeviceAuthenticationViewModel>();

            builder.Services.AddTransient<OpenIdAuthenticationRouter>();
            builder.Services.AddTransient<OpenIdAuthenticationPage>();
            builder.Services.AddTransient<OpenIdAuthenticationViewModel>();

            builder.Services.AddTransient<CasesAndIncidentsPage>();
            builder.Services.AddTransient<CasesAndIncidentsViewModel>();

            return builder.Build();
        }
    }
}

