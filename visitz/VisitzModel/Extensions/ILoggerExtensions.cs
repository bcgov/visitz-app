using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace VisitzModel.Extensions;

public static partial class ILoggerExtensions
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
        if (logger.IsEnabled(LogLevel.Trace))
        {
            Interlocked.Increment(ref TraceCount);
            LogTraceMethod(logger, TraceCount, instance.GetType().Name, callerName, message);
        }
#endif
    }

    public static void LogException<T>(this ILogger<T> logger, Exception exception, string? message = null)
    {
        LogException((ILogger)logger, exception, message);
    }

    public static void LogException(this ILogger logger, Exception exception, string? message = null)
    {
        LogError(logger, exception, message ?? exception.Message);
    }

#if DEBUG
    [LoggerMessage(Level = LogLevel.Trace, Message = "{traceCount}|{instance}.{caller} {message}")]
    static partial void LogTraceMethod(ILogger logger, int traceCount, string instance, string caller, string message);
#endif

    [LoggerMessage(EventId = (int)LogLevel.Error, Level = LogLevel.Error, Message = "{message}")]
    static partial void LogError(ILogger logger, Exception exception, string message);
}
