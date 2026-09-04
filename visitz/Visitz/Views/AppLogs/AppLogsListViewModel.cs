using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Logging;

namespace Visitz.Views.AppLogs;

public partial class AppLogsListViewModel : VisitzViewModel
{
    bool _disposed;

    Realm? _logRealm;

    [ObservableProperty]
    public partial IEnumerable<LogEntry> Logs { get; set; } = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _logRealm = await VisitzRealms.GetLogRealmAsync();

        Logs = _logRealm.All<LogEntry>().OrderByDescending(log => log.Timestamp);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Logs = [];

            _logRealm?.Dispose();
            _logRealm = null;

            _disposed = true;
        }
        base.Dispose(disposing);
    }
}
