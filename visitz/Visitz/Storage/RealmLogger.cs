using Microsoft.Extensions.Logging;
using VisitzModel.Models.Logging;

namespace Visitz.Storage;

public class RealmLogger(string categoryName, Func<RealmLoggerConfiguration> getCurrentConfig) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        RealmLoggerConfiguration config = getCurrentConfig();
        return logLevel >= config.MinimumLogLevel && logLevel <= config.MaximumLogLevel;
    }

    public async void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
            return;

        LogEntry log = new()
        {
            LogLevel = logLevel,
            Message = formatter(state, exception),
            Source = categoryName,
            Timestamp = DateTimeOffset.UtcNow,
        };

        using var logRealm = await VisitzRealms.GetLogRealmAsync();
        await logRealm.WriteAsync(() => logRealm.Add(log));
    }
}
