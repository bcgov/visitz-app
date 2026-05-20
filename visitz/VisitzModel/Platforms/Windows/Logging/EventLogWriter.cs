using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace VisitzModel.Platforms.Windows.Logging;

public static class EventLogWriter
{
    static readonly string _logSource = "Application";
    static readonly int _defaultEventId = 1;
    static readonly int _maxLogLength = 32766; // Event Viewer limitation

    public static void WriteEntry(
        LogLevel logLevel,
        string message,
        string categoryName,
        int? eventId = null,
        Exception? exception = null
    )
    {
        string level = logLevel.ToString().ToUpperInvariant();
        Assembly? assembly = Assembly.GetEntryAssembly();

        string outputMessage =
            @$"{assembly?.GetName().Name ?? "NULL ASSEMBLY ERROR"} {AppInfo.Current.VersionString}
Level: {level}
Category: {categoryName}
Message: {message}";

        if (exception != null)
            outputMessage += "\nStack trace: " + exception.ToString();

        if (outputMessage.Length > _maxLogLength)
            outputMessage = outputMessage[.._maxLogLength];

        EventLog.WriteEntry(_logSource, outputMessage, ConvertLogLevel(logLevel), eventId ?? _defaultEventId);
    }

    static EventLogEntryType ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            <= LogLevel.Information => EventLogEntryType.Information,
            LogLevel.Warning => EventLogEntryType.Warning,
            <= LogLevel.Critical => EventLogEntryType.Error,
            _ => throw new NotImplementedException(logLevel.ToString()),
        };
    }
}
