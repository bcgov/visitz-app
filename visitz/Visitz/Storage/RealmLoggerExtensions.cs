using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Visitz.Storage;

public static class RealmLoggerExtensions
{
    public static ILoggingBuilder AddRealmLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, RealmLoggerProvider>());

        return builder;
    }

    public static ILoggingBuilder AddRealmLogger(
        this ILoggingBuilder builder,
        Action<RealmLoggerConfiguration> configure
    )
    {
        builder.AddRealmLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}
