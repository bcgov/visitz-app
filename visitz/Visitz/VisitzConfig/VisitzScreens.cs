using Visitz.Device;
using Visitz.Pages;
using Visitz.ViewModels;
using Visitz.ViewModels.Entity;
using Visitz.Views;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;
using Visitz.Views.Entity;
using Visitz.Views.Notes;

namespace Visitz.VisitzConfig
{
    public static class VisitzScreens
    {
        public static MauiAppBuilder ConfigureVisitzScreens(this MauiAppBuilder builder)
        {
			builder.Services.AddSingleton<RootPage>();
            builder.Services.AddSingleton<RootViewModel>();

            builder.Services.AddSingleton<NavRailViewModel>();

            builder.Services.AddSingleton<CaseloadContainerView>();
            builder.Services.AddSingleton<WatermarkView>();

            builder.Services.AddSingleton<CaseloadView>();
            builder.Services.AddSingleton<CaseloadViewModel>();

            builder.Services.AddTransient<DeviceAuthenticator>();
            builder.Services.AddTransient<AppLockPage>();
            builder.Services.AddTransient<AppLockViewModel>();

			builder.Services.AddTransient<WebViewPage>();
			builder.Services.AddTransient<WebViewModel>();

			builder.Services.AddTransient<EntityNavView>();
			builder.Services.AddTransient<EntityNavViewModel>();

            builder.Services.AddTransient<EntityContainerView>();
            builder.Services.AddTransient<EntityContainerViewModel>();

            builder.Services.AddTransient<EntityDetailsView>();
            builder.Services.AddTransient<EntityDetailsViewModel>();

            builder.Services.AddTransient<EntityContactsView>();
            builder.Services.AddTransient<EntityContactsViewModel>();

            builder.Services.AddTransient<EntityNotesView>();
            builder.Services.AddTransient<EntityNotesViewModel>();

            builder.Services.AddTransient<EntitySafetyAssessView>();
            builder.Services.AddTransient<EntitySafetyAssessViewModel>();

            builder.Services.AddTransient<PublishPage>();
            builder.Services.AddTransient<NotePublishViewModel>();
            builder.Services.AddTransient<SafetyAssessmentPublishViewModel>();

            builder.Services.AddTransient<NoteEntryView>();
            builder.Services.AddTransient<NoteEntryViewModel>();

            builder.Services.AddTransient<DebugOptionsPage>();
            builder.Services.AddTransient<DebugOptionsView>();
            builder.Services.AddTransient<DebugOptionsViewModel>();

            builder.Services.AddTransient<SessionPage>();
            builder.Services.AddTransient<SessionViewModel>();

            return builder;
        }
    }
}
