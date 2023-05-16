using Visitz.Routers;
using Visitz.Services;
using Visitz.ViewModels;
using Visitz.Views;

namespace Visitz.VisitzConfig
{
    public static class VisitzScreens
    {
        public static MauiAppBuilder ConfigureVisitzScreens(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<AppLockPage>();
            builder.Services.AddTransient<AppLockViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

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
