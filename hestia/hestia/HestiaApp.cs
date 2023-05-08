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
            builder.UseMauiApp<App>();

            HestiaLocalization.ConfigureHestiaLocalization(builder);
            HestiaFonts.ConfigureHestiaFonts(builder);
            HestiaAuth.ConfigureHestiaAuth(builder);
            HestiaApiConfig.ConfigureHestiaApi(builder);
            HestiaLogging.ConfigureHestiaLogging(builder);
            HestiaScreens.ConfigureHestiaScreens(builder);

            return builder.Build();
        }
    }
}

