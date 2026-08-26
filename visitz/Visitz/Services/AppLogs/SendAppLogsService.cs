using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using Visitz.Views.Debugging;
using VisitzApi;
using VisitzApi.Models.AppLogs;
using VisitzModel.Models.Logging;
using VisitzModel.Storage;

namespace Visitz.Services.AppLogs;

internal class SendAppLogsService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
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

        IList<LogEntry> savedLogs = logRealm.All<LogEntry>().ToList();
        IList<AppLogJson> uploadLogs = savedLogs.Select(ToAppLogJson).ToList();

        if (!DebugOptions.Default.DryFireSendAppLogs)
            await Vpi.SendAppLogs(uploadLogs);

        if (!DebugOptions.Default.KeepLogsAfterSending)
        {
            await logRealm.WriteAsync(() =>
            {
                foreach (var log in savedLogs)
                    logRealm.Remove(log);
            });
        }

        ResultCode = Result.Successful;
    }

    AppLogJson ToAppLogJson(LogEntry log)
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
            Message = new { Content = log.Message },
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

    bool ShouldRun()
    {
#if DEBUG
        bool isDebug = true;
#else
        bool isDebug = false;
#endif
        string name = nameof(DebugOptions.Default.RunAppLogsServiceInDebug);
        bool runInDebug = DebugOptions.Default.RunAppLogsServiceInDebug;

#if DEBUG
        Logger.LogDebug(nameof(ShouldRun) + $" -> isDebug: {isDebug}, {name}: {runInDebug}");
#endif

        return !isDebug || runInDebug;
    }
}
