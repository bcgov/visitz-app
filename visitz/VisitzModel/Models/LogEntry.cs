using System.Globalization;
using Realms;
using VisitzModel.Formats;

namespace VisitzModel.Models
{
    public partial class LogEntry : IRealmObject
    {
        public string Type { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public string Timestamp { get; set; }

        public static async Task AddLogEntry(string logType, string logMessage, string logSource, string timeStamp, Realm realm)
        {
            var logEntry = new LogEntry
            {
                Type = logType,
                Message = logMessage,
                Source = logSource,
                Timestamp = timeStamp
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
    }
}
