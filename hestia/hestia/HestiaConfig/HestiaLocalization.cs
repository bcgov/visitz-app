using hestia.Services.Localization;

namespace hestia.HestiaConfig
{
    public static class HestiaLocalization
    {
        public static MauiAppBuilder ConfigureHestiaLocalization(this MauiAppBuilder builder)
        {
            // This service is needed to inject IStringLocalizer into LocalizeExtension
            builder.Services.AddLocalization();

            builder.Services.AddSingleton<LocalizeExtension>();

            return builder;
        }
    }
}
