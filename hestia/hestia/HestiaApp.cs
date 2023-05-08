using System;
using Microsoft.Extensions.Logging;
using hestia.Extensions;
using hestia.HestiaConfig;

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
            builder.UseMauiApp<App>()
                .RegisterServices(); // Dependency Injection Setup
            
            HestiaFonts.ConfigureFonts(builder);
            HestiaAuth.ConfigureAuth(builder);

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}

