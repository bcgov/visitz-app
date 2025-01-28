using Visitz.Services;
using Visitz.Services.Notes;
using Visitz.Services.Visits;
using VisitzModel.Storage;

namespace Visitz.VisitzConfig
{
    public static class VisitzServicesConfig
    {
        public static MauiAppBuilder ConfigureVisitzApiServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ServiceHandler>();

            builder.Services.AddTransient<GetCaseloadService>();
            builder.Services.AddTransient<GetNotesService>();
            builder.Services.AddTransient<GetNotesForRangeService>();
            builder.Services.AddTransient<GetAllDataForOfflineService>();
            builder.Services.AddTransient<SubmitNoteService>();
            builder.Services.AddTransient<SubmitAndGetNotesService>();
            builder.Services.AddTransient<SubmitSafetyAssessmentService>();
			builder.Services.AddTransient<SubmitAttachmentService>();
            builder.Services.AddTransient<GetVisitsService>();
            builder.Services.AddTransient<GetVisitsByRangeService>();

            return builder;
        }

		public static MauiAppBuilder ConfigureVisitzUtilities(this MauiAppBuilder builder)
		{
			builder.Services.AddSingleton(_ => new LastUpdatedPrefs(Preferences.Default));

			return builder;
		}
    }
}
