using visitz.VisitzConfig;

namespace visitz
{
    /// <summary>
    /// Application setup and configurations. (Separation of Concerns)
    /// </summary>
    public class VisitzApp
    {
        public static MauiApp Create()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureVisitzLocalization()
                .ConfigureVisitzFonts()
                .ConfigureVisitzAuth()
                .ConfigureVisitzApi()
                .ConfigureVisitzLogging()
                .ConfigureVisitzScreens();

            return builder.Build();
        }
    }
}

