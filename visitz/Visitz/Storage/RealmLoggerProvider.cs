using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Logger = Microsoft.Extensions.Logging.ILogger;

namespace Visitz.Storage;

public partial class RealmLoggerProvider : ILoggerProvider
{
    bool _disposed;

    readonly IDisposable? _onChangeToken;

    readonly ConcurrentDictionary<string, RealmLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    RealmLoggerConfiguration _currentConfig;

    public RealmLoggerProvider(IOptionsMonitor<RealmLoggerConfiguration> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }

    public Logger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, new RealmLogger(categoryName, GetCurrentConfig));
    }

    RealmLoggerConfiguration GetCurrentConfig() => _currentConfig;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _loggers.Clear();
                _onChangeToken?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
