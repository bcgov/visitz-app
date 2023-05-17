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

            builder.Services.AddSingleton<CaseloadPage>();
            builder.Services.AddSingleton<CaseloadViewModel>();

            builder.Services.AddTransient<NotesPage>();
            builder.Services.AddTransient<NotesViewModel>();

            builder.Services.AddTransient<CaseloadItemDetailsPage>();
            builder.Services.AddTransient<CaseloadItemDetailsViewModel>();

            return builder;
        }
    }
}
