using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace VisitzModel.Platforms.Windows.Logging;

public static class EventLogWriter
{
    static readonly string _logSource = "Application";
    static readonly int _defaultEventId = 1;

    public static void WriteEntry(LogLevel logLevel, string message, string categoryName, int? eventId = null, Exception exception = null)
    {
        string level = logLevel.ToString().ToUpperInvariant();
        var assembly = Assembly.GetEntryAssembly();

        string outputMessage =
@$"{assembly.GetName().Name} {AppInfo.Current.VersionString}
Level: {level}
Category: {categoryName}
Message: {message}";

        if (exception != null)
            outputMessage += "\nStack trace: " + exception.ToString();

        EventLog.WriteEntry(_logSource, outputMessage, ConvertLogLevel(logLevel), eventId ?? _defaultEventId);
    }

    static EventLogEntryType ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            <= LogLevel.Information => EventLogEntryType.Information,
            LogLevel.Warning => EventLogEntryType.Warning,
            <= LogLevel.Critical => EventLogEntryType.Error,
            _ => throw new NotImplementedException(logLevel.ToString())
        };
    }
}
