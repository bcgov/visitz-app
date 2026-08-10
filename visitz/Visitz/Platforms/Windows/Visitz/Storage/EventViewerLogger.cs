using Microsoft.Extensions.Logging;
using VisitzModel.Platforms.Windows.Logging;

namespace Visitz.Platforms.Windows.Visitz.Storage;

internal class EventViewerLogger(string categoryName) : ILogger
{
    static readonly LogLevel _defaultMinimumLevel =
#if DEBUG
    LogLevel.Trace;
#else
    LogLevel.Warning;
#endif

    readonly string _categoryName = categoryName;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && logLevel >= _defaultMinimumLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
            return;

        EventLogWriter.WriteEntry(logLevel, formatter(state, exception), _categoryName, eventId.Id, exception);
    }
}
