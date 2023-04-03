using System;
using Microsoft.Extensions.Logging;
using hestia.Extensions;

namespace hestia
{
    /// <summary>
    /// Application setup and configurations. (Separation of Concerns)
    /// </summary>
    public class HestiaApp
    {
        public static MauiApp Create()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterServices() // Dependency Injection Setup
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("BCSans-Bold.ttf", "BCSansBold");
                    fonts.AddFont("BCSans-BoldItalic.ttf", "BCSansBoldItalic");
                    fonts.AddFont("BCSans-Italic.ttf", "BCSansItalic");
                    fonts.AddFont("BCSans-Regular.ttf", "BCSansRegular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}

