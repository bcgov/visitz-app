using hestia.Services.Localization;

namespace hestia.HestiaConfig
{
    public class HestiaLocalization
    {
        public static MauiAppBuilder ConfigureLocalization(MauiAppBuilder builder)
        {
            // This service is needed to inject IStringLocalizer into LocalizeExtension
            builder.Services.AddLocalization();

            builder.Services.AddSingleton<LocalizeExtension>();

            return builder;
        }
    }
}
