using visitz.Services.Localization;

namespace visitz.VisitzConfig
{
    public static class VisitzLocalization
    {
        public static MauiAppBuilder ConfigureVisitzLocalization(this MauiAppBuilder builder)
        {
            // This service is needed to inject IStringLocalizer into LocalizeExtension
            builder.Services.AddLocalization();

            builder.Services.AddSingleton<LocalizeExtension>();

            return builder;
        }
    }
}
