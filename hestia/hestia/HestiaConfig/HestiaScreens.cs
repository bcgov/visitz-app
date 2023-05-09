using hestia.Routers;
using hestia.Services;
using hestia.ViewModels;
using hestia.Views;

namespace hestia.HestiaConfig
{
    public static class HestiaScreens
    {
        public static MauiAppBuilder ConfigureHestiaScreens(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<LandingPage>();
            builder.Services.AddSingleton<LandingRouter>();
            builder.Services.AddSingleton<LandingViewModel>();

            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<DeviceAuthenticationRouter>();
            builder.Services.AddTransient<DeviceAuthenticationPage>();
            builder.Services.AddTransient<DeviceAuthenticationViewModel>();

            builder.Services.AddTransient<OpenIdAuthenticationRouter>();
            builder.Services.AddTransient<OpenIdAuthenticationPage>();
            builder.Services.AddTransient<OpenIdAuthenticationViewModel>();

            builder.Services.AddTransient<CasesAndIncidentsRouter>();
            builder.Services.AddTransient<CasesAndIncidentsPage>();
            builder.Services.AddTransient<CaseloadViewModel>();

            builder.Services.AddTransient<CaseNotesRouter>();
            builder.Services.AddTransient<CaseNotesPage>();
            builder.Services.AddTransient<CaseNotesViewModel>();

            builder.Services.AddTransient<CaseIncidentDetailsPage>();
            builder.Services.AddTransient<CaseIncidentDetailsViewModel>();

            return builder;
        }
    }
}
