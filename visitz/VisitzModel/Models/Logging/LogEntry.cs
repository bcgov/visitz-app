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
        set
        {
            LogLevelInt = (int)value;
            LevelText = value.ToString();
        }
    }

    public string Message { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public DateTimeOffset Timestamp { get; set; }

    public override string ToString()
    {
        return $"[{Timestamp}] {Source}: {LevelText}: {Message}";
    }
}
