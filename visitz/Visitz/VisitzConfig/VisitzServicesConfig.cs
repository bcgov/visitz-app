using Visitz.Services;

namespace Visitz.VisitzConfig
{
    public static class VisitzServicesConfig
    {
        public static MauiAppBuilder ConfigureVisitzServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ServiceHandler>();

            builder.Services.AddTransient<GetCaseloadService>();
            builder.Services.AddTransient<GetNotesService>();

            return builder;
        }
    }
}
