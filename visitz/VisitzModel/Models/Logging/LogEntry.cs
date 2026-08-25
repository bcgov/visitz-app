using Microsoft.Extensions.Logging;
using Realms;

namespace VisitzModel.Models.Logging;

public partial class LogEntry : IRealmObject
{
    [MapTo("Type")]
    public string LevelText { get; set; } = string.Empty;

    private int LogLevelInt { get; set; }

    public LogLevel LogLevel
    {
        get => (LogLevel)LogLevelInt;
        set => LogLevelInt = (int)value;
    }

    public string Message { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public DateTimeOffset Timestamp { get; set; }

    public static async Task AddLogEntry(
        string logType,
        string logMessage,
        string logSource,
        DateTimeOffset timeStamp,
        Realm realm
    )
    {
        var logEntry = new LogEntry
        {
            Type = logType,
            Message = logMessage,
            Source = logSource,
            Timestamp = timeStamp,
        };
        try
        {
            await realm.WriteAsync(() => realm.Add(logEntry));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public override string ToString()
    {
        return $"[{Timestamp}] {Source}: {LevelText}: {Message}";
    }
}
