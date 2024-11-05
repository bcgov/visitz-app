using MetroLog;
using MetroLog.Targets;
using Microsoft.Extensions.Logging;
using VisitzModel.Models;
using Logger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Visitz.Storage
{
    public class RealmAsyncTarget : AsyncTarget, ILoggerProvider
    {
        public RealmAsyncTarget() : base(default) { }

        public Logger CreateLogger(string categoryName)
        {
            return new RealmLogger(this, categoryName);
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Warning;
        }

        protected override async Task<LogWriteOperation> WriteAsyncCore(LogWriteContext context, LogEventInfo logEvent)
        {
            var realm = await VisitzRealms.GetLogRealmAsync();
            var timestamp = RealmLogger.GetTimestamp(logEvent.TimeStamp.DateTime);

            await LogEntry.AddLogEntry(
                logEvent.Level.ToString(),
                logEvent.Message,
                logEvent.Logger,
                timestamp,
                realm
            );
            return new LogWriteOperation();
        }

        public async Task WriteLogAsync(LogWriteContext context, LogEventInfo logEvent)
        {
            await WriteAsyncCore(context, logEvent);
        }
    }
}
