using Microsoft.Extensions.Logging;

namespace Visitz.Platforms.Windows.Visitz.Storage;

internal partial class EventViewerLoggingProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new EventViewerLogger(categoryName);
    }

    public void Dispose() { }
}
