using Realms;

namespace VisitzModel.Models.Logging;

public partial class LogEntry : IRealmObject
{
    public string Type { get; set; } = string.Empty;

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

    public static List<LogEntry> GetLogEntries(Realm realm)
    {
        return realm.All<LogEntry>().ToList();
    }

    public override string ToString()
    {
        return $"[{Timestamp}] {Type}: {Message} (Source: {Source})";
    }
}
