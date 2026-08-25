using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
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
        using Realm logRealm = await VisitzRealms.GetLogRealmAsync();

        IEnumerable<AppLogJson> logs = logRealm.All<LogEntry>().AsEnumerable().Select(ToAppLogJson);

        // TODO: send logs upstream

        // TODO: Remove sent logs from DB
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
            Message = log.Message,
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
}
