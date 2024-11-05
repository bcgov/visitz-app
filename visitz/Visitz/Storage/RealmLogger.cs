using MetroLog;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Logger = Microsoft.Extensions.Logging.ILogger;
using VisitzModel.Formats;
using System.Globalization;

namespace Visitz.Storage
{
    public class RealmLogger : Logger
    {
        private readonly RealmAsyncTarget _target;
        private readonly string _categoryName;

        public RealmLogger(RealmAsyncTarget target, string categoryName)
        {
            _target = target;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_target != null && _target.IsEnabled(logLevel))
            {
                var message = formatter(state, exception);
                var logEvent = new LogEventInfo((MetroLog.LogLevel)logLevel, _categoryName, message, exception)
                {
                    TimeStamp = DateTimeOffset.UtcNow
                };
                await _target.WriteLogAsync(new LogWriteContext(), logEvent);
            }
        }
    }
}
