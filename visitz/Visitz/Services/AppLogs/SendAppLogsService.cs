using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using Visitz.Views.Debugging;
using VisitzApi;
using VisitzApi.Models.AppLogs;
using VisitzModel.Formats;
using VisitzModel.Models.Logging;
using VisitzModel.Storage;

namespace Visitz.Services.AppLogs;

internal class SendAppLogsService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    // Max limit 5MB with a buffer amount to account for extra JSON characters
    static readonly int s_maxUploadSize = (int)(5 * Sizes.MB * 0.90d);

    public static string MakeId()
    {
        return nameof(SendAppLogsService);
    }

    public static StartServiceMessage MakeStartMessage()
    {
        return new() { ServiceId = MakeId(), ServiceType = typeof(SendAppLogsService) };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunApiServiceAsync()
    {
        if (!ShouldRun())
        {
            ResultCode = Result.Cancelled;
            return;
        }

        using Realm logRealm = await VisitzRealms.GetLogRealmAsync();
        IList<LogEntry> savedLogs = logRealm.All<LogEntry>().OrderBy(log => log.Timestamp).ToList();

        if (savedLogs.Count <= 0)
        {
            ResultCode = Result.Cancelled;
            ResultMessage = "No logs to send";
            return;
        }

        (ResultCode, ResultMessage) = await UploadLogs(savedLogs, logRealm);
    }

    async Task<(Result, string?)> UploadLogs(IList<LogEntry> savedLogs, Realm logRealm)
    {
        // To keep this service simpler, we'll only upload within
        // s_maxUploadSize of the oldest logs per service run. We will assume
        // the app does not produce more logs than can be uploaded and
        // truncated per upload interval (at the time of writing: caseload
        // refreshes).
        var (onePageLogs, uploadLogs) = LimitAndMapLogs(savedLogs);

        Result? resultCode = null;
        string? resultMessage = null;

        if (!DebugOptions.Default.DryFireSendAppLogs)
        {
            HttpResponseMessage response = await Vpi.SendAppLogs(uploadLogs);

            resultCode = response.IsSuccessStatusCode ? Result.Successful : Result.Error;
            resultMessage = $"HTTP {response.StatusCode} -> {await response.Content.ReadAsStringAsync()}";
        }

        if (!DebugOptions.Default.KeepLogsAfterSending)
        {
            await logRealm.WriteAsync(() =>
            {
                foreach (var log in onePageLogs)
                    logRealm.Remove(log);
            });
        }

        return (resultCode ?? Result.Successful, resultMessage);
    }

    static (List<LogEntry>, List<AppLogJson>) LimitAndMapLogs(IList<LogEntry> savedLogs)
    {
        int totalSize = 0;
        int index = 0;
        List<LogEntry> onePageLogs = [];
        List<AppLogJson> uploadLogs = [];

        while (totalSize < s_maxUploadSize && index < savedLogs.Count)
        {
            LogEntry log = savedLogs[index];
            AppLogJson logJson = ToAppLogJson(log);

            string serializedLog = JsonSerializer.Serialize(logJson);
            int nextSize = Encoding.UTF8.GetByteCount(serializedLog);

            if (totalSize + nextSize < s_maxUploadSize)
            {
                onePageLogs.Add(log);
                uploadLogs.Add(logJson);
                totalSize += nextSize;
            }
            index++;
        }

        return (onePageLogs, uploadLogs);
    }

    static AppLogJson ToAppLogJson(LogEntry log)
    {
        return new()
        {
            AppVersion = AppInfo.Current.VersionString,
            Device = new()
            {
                Idiom = DeviceInfo.Current.Idiom.ToString(),
                Manufacturer = DeviceInfo.Current.Manufacturer,
                Model = DeviceInfo.Current.Model,
                OSVersion = DeviceInfo.Current.Version.ToString(),
                Platform = DeviceInfo.Current.Platform.ToString(),
            },
            DotnetRuntime = Environment.Version.ToString(),
            Level = MapType(log.LogLevel),
            Message = MakeMessage(log),
            SourceName = log.Source,
            AppTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };
    }

    static AppLogLevel MapType(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => AppLogLevel.Debug,
            LogLevel.Information => AppLogLevel.Info,
            LogLevel.Warning => AppLogLevel.Warning,
            LogLevel.Error => AppLogLevel.Error,
            LogLevel.Critical => AppLogLevel.Critical,
            LogLevel.Trace => AppLogLevel.Verbose,
            _ => throw new InvalidOperationException($"Unsupported LogLevel '{level}'"),
        };
    }

    static object MakeMessage(LogEntry log)
    {
        return log.LogLevel switch
        {
            LogLevel.Critical or LogLevel.Error => new { Error = log.Message },
            LogLevel.Warning => new { Warning = log.Message },
            _ => new { Content = log.Message },
        };
    }

    // We don't want to spam upstream with test environment logs during
    // development, but also need a way to override that to test this feature.
    bool ShouldRun()
    {
#if DEBUG
        bool isDebug = true;
#else
        bool isDebug = false;
#endif
        string name = nameof(DebugOptions.Default.RunAppLogsServiceInDebug);
        bool runInDebug = DebugOptions.Default.RunAppLogsServiceInDebug;
        bool result = !isDebug || runInDebug;

#if DEBUG
        Logger.LogDebug(nameof(ShouldRun) + $"? {result} -> isDebug: {isDebug}, {name}: {runInDebug}");
#endif

        return result;
    }
}
