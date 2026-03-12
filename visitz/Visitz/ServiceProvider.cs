namespace Visitz
{
    /// <summary>
    /// Alternate service for Dependency Injection. Use this when there is a need to bypass constructor injection.
    /// </summary>
    public class ServiceProvider
    {
        public static TService GetService<TService>() => Current.GetService<TService>();

        public static object GetService(Type serviceType) => Current.GetService(serviceType);

        public static IServiceProvider Current => IPlatformApplication.Current.Services;
    }
}

