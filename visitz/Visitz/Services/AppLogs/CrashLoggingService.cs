using System.Text.Json;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzModel.Models.Logging;

namespace Visitz.Services.AppLogs;

public class CrashLoggingService(CrashFileStore crashFileStore) : VisitzService
{
    public static StartServiceMessage MakeStartMessage()
    {
        return new() { ServiceId = MakeId(), ServiceType = typeof(CrashLoggingService) };
    }

    public static string MakeId()
    {
        return nameof(CrashLoggingService);
    }

    public override string GetId()
    {
        return MakeId();
    }

    public async Task ProcessPendingAsync()
    {
        using Realm logRealm = await VisitzRealms.GetLogRealmAsync();
        foreach (string filePath in crashFileStore.GetPendingCrashFiles())
        {
            try
            {
                string json = await File.ReadAllTextAsync(filePath);

                CrashRecord? crash = JsonSerializer.Deserialize<CrashRecord>(json);

                if (crash == null)
                {
                    Logger.LogWarning("Unable to deserialize crash file {FilePath}", filePath);

                    continue;
                }

                var logEntry = new LogEntry
                {
                    LogLevel = LogLevel.Critical,
                    Source = GetId(),
                    Timestamp = crash.TimestampUtc,
                    Message = crash.ExceptionText,
                };

                await logRealm.WriteAsync(() =>
                {
                    logRealm.Add(logEntry);
                });

                // Successfully processed, so remove the temporary crash file
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process crash file {FilePath}", filePath);
            }
        }
    }

    protected override async Task RunServiceAsync()
    {
        await ProcessPendingAsync();
        ResultCode = Result.Successful;
    }
}
