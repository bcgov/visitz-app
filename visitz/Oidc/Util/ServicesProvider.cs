namespace Oidc.Util;

public class ServicesProvider
{
    public static TService GetService<TService>() => Current.GetService<TService>();

    public static object GetService(Type serviceType) => Current.GetService(serviceType);

    public static IServiceProvider Current => IPlatformApplication.Current.Services;
}
