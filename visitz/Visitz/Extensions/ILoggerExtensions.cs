using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Visitz.Extensions;

public static class ILoggerExtensions
{
    static int TraceCount;

    public static void TraceMethod<T>(this ILogger<T> logger, object instance, [CallerMemberName] string callerName = "")
    {
        TraceMethod((ILogger)logger, instance, callerName);
    }

    public static void TraceMethod(
        this ILogger logger,
        object instance,
        string message = "",
        [CallerMemberName] string callerName = "")
    {
#if DEBUG
        logger.LogTrace(
            "{traceCount}|{instance}.{caller} {message}",
            TraceCount++,
            instance.GetType().Name,
            callerName,
            message);
#endif
    }
}