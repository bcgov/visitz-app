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

            HestiaLocalization.ConfigureLocalization(builder);
            HestiaFonts.ConfigureFonts(builder);
            HestiaAuth.ConfigureAuth(builder);
            HestiaApiConfig.ConfigureApi(builder);
            HestiaLogging.ConfigureLogging(builder);

            return builder.Build();
        }
    }
}

