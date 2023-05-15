namespace Visitz.Services.MAUI
{
    /// <summary>
    /// Alternate service for Dependency Injection. Use this when there is a need to bypass constructor injection.
    /// </summary>
    public class ServiceProvider
    {
        public static TService GetService<TService>()
            => Current.GetService<TService>();

        public static IServiceProvider Current
            =>
#if WINDOWS10_0_17763_0_OR_GREATER
				MauiWinUIApplication.Current.Services;
#elif ANDROID
                MauiApplication.Current.Services;
#elif IOS || MACCATALYST
				MauiUIApplicationDelegate.Current.Services;
#else
				null;
#endif
    }
}

