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

            builder.Services.AddTransient<CaseloadRouter>();
            builder.Services.AddTransient<CaseloadPage>();
            builder.Services.AddTransient<CaseloadViewModel>();

            builder.Services.AddTransient<NotesRouter>();
            builder.Services.AddTransient<NotesPage>();
            builder.Services.AddTransient<NotesViewModel>();

            builder.Services.AddTransient<CaseIncidentDetailsPage>();
            builder.Services.AddTransient<CaseIncidentDetailsViewModel>();

            return builder;
        }
    }
}
