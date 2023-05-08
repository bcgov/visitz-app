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
            builder
                .UseMauiApp<App>()
                .ConfigureHestiaLocalization()
                .ConfigureHestiaFonts()
                .ConfigureHestiaAuth()
                .ConfigureHestiaApi()
                .ConfigureHestiaLogging()
                .ConfigureHestiaScreens();

            return builder.Build();
        }
    }
}

