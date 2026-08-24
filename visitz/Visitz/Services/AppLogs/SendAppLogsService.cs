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
            Level = MapType(log.Type),
            Message = log.Message,
            SourceName = log.Source,
            AppTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };
    }

    static AppLogLevel MapType(string type)
    {
        if (type == LogLevel.Trace.ToString())
            return AppLogLevel.Verbose;
        else if (type == LogLevel.Debug.ToString())
            return AppLogLevel.Debug;
        else if (type == LogLevel.Information.ToString())
            return AppLogLevel.Info;
        else if (type == LogLevel.Warning.ToString())
            return AppLogLevel.Warning;
        else if (type == LogLevel.Error.ToString())
            return AppLogLevel.Error;
        else if (type == LogLevel.Critical.ToString())
            return AppLogLevel.Critical;
        else
            return AppLogLevel.Unknown;
    }
}
