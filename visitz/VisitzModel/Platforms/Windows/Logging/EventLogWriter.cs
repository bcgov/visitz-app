using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace VisitzModel.Platforms.Windows.Logging;

public static class EventLogWriter
{
    static readonly string _logSource = "Application";
    static readonly int _defaultEventId = 1;

    // Event Viewer limitation is 32766, but we'll go lower to as a more
    // simple way to handle boundaries instead of calculating UTF8 byte
    // boundaries.
    static readonly int _maxLogLength = 20000;

    public static void WriteEntry(
        LogLevel logLevel,
        string message,
        string categoryName,
        int? eventId = null,
        Exception? exception = null
    )
    {
        string id = Guid.NewGuid().ToString();
        string level = logLevel.ToString().ToUpperInvariant();
        Assembly? assembly = Assembly.GetEntryAssembly();

        string header =
            @$"{assembly?.GetName().Name ?? "NULL ASSEMBLY ERROR"} {AppInfo.Current.VersionString}
Log ID: {id}
Level: {level}
Category: {categoryName}";

        if (exception != null)
            message += "\nStack trace: " + exception.ToString();

        foreach (char[] messageChunk in message.Chunk(_maxLogLength))
            EventLog.WriteEntry(
                _logSource,
                $"{header}:\n\n{new string(messageChunk)}",
                ConvertLogLevel(logLevel),
                eventId ?? _defaultEventId
            );
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
