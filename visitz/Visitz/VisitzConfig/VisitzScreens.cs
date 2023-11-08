using Visitz.Authentication;
using Visitz.Pages;
using Visitz.ViewModels;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;

namespace Visitz.VisitzConfig
{
    public static class VisitzScreens
    {
        public static MauiAppBuilder ConfigureVisitzScreens(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<CaseloadContainerView>();

            builder.Services.AddSingleton<CaseloadView>();
            builder.Services.AddSingleton<CaseloadViewModel>();

            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<AppLockPage>();
            builder.Services.AddTransient<AppLockViewModel>();

            builder.Services.AddTransient<NotesPage>();
            builder.Services.AddTransient<NotesViewModel>();

            builder.Services.AddTransient<NoteDetailsPage>();
            builder.Services.AddTransient<NoteDetailsViewModel>();

            builder.Services.AddTransient<NoteEntryPage>();
            builder.Services.AddTransient<NoteEntryViewModel>();

            builder.Services.AddTransient<NotePublishPage>();
            builder.Services.AddTransient<NotePublishViewModel>();

            builder.Services.AddTransient<CaseloadItemDetailsPage>();
            builder.Services.AddTransient<CaseloadItemDetailsViewModel>();

            builder.Services.AddTransient<DebugOptionsPage>();
            builder.Services.AddTransient<DebugOptionsView>();
            builder.Services.AddTransient<DebugOptionsViewModel>();

            builder.Services.AddTransient<SessionPage>();
            builder.Services.AddTransient<SessionViewModel>();

            return builder;
        }
    }
}
