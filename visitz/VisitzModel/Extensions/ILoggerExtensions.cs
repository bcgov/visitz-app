using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace VisitzModel.Extensions;

public static class ILoggerExtensions
{
#if DEBUG
    static int TraceCount;
#endif

    public static void TraceMethod<T>(
        this ILogger<T> logger,
        object instance,
        [CallerMemberName] string callerName = ""
    )
    {
        TraceMethod((ILogger)logger, instance, callerName);
    }

    public static void TraceMethod(
        this ILogger logger,
        object instance,
        string message = "",
        [CallerMemberName] string callerName = ""
    )
    {
#if DEBUG
        logger.LogTrace(
            "{traceCount}|{instance}.{caller} {message}",
            TraceCount++,
            instance.GetType().Name,
            callerName,
            message
        );
#endif
    }

    public static void LogException<T>(this ILogger<T> logger, Exception exception, string? message = null)
    {
        LogException((ILogger)logger, exception, message);
    }

    public static void LogException(this ILogger logger, Exception exception, string? message = null)
    {
        logger.LogError(exception, (message ?? exception.Message) + " -> " + exception.ToString());
    }
}
