using Microsoft.Extensions.Logging;

namespace Visitz.Storage;

public class RealmLoggerConfiguration
{
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Trace;

    public LogLevel MaximumLogLevel { get; set; } = LogLevel.Critical;
}
