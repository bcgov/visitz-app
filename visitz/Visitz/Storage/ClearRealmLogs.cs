using Realms;
using VisitzModel.Models;

namespace Visitz.Storage;

public static class ClearRealmLogs
{
    private static readonly int MaxNumberOfLogs = 1000;
    private static readonly int MaxDaysOfLogsLife = 14;

    private static void DeleteLogs(Realm realm, IEnumerable<LogEntry> logsToDelete)
    {
        foreach (var log in logsToDelete.ToList())
            realm.Remove(log);
    }

    public static async Task ClearLogData()
    {
        using var logRealm = await VisitzRealms.GetLogRealmAsync();

        await logRealm.WriteAsync(() =>
        {
            var allLogs = logRealm.All<LogEntry>().OrderByDescending(log => log.Timestamp);
            var logsToDelete = allLogs.ToList().Skip(MaxNumberOfLogs);
            DeleteLogs(logRealm, logsToDelete);

            var twoWeeksAgo = DateTimeOffset.UtcNow.AddDays(-MaxDaysOfLogsLife);
            var twoWeeksOldLog = allLogs.Where(log => log.Timestamp <= twoWeeksAgo);
            DeleteLogs(logRealm, twoWeeksOldLog);
        });
    }
}
