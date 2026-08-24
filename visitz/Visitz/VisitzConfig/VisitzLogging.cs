using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using Visitz.Storage;
using MetroLogLevel = MetroLog.LogLevel;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;
#if WINDOWS
using Visitz.Platforms.Windows.Visitz.Storage;
#endif

namespace Visitz.VisitzConfig;

public static class VisitzLogging
{
    public static MauiAppBuilder ConfigureVisitzLogging(this MauiAppBuilder builder)
    {
        // IStringLocalizer appears to be dependent on a logging service
        builder.Services.AddLogging();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder
            .AddConsoleLogger(options =>
            {
                options.MaxLevel = (MicrosoftLogLevel?)MetroLogLevel.Fatal;
            });

        builder.Logging.Services.AddSingleton<ILoggerProvider, RealmAsyncTarget>();

#if WINDOWS
        builder.Logging.Services.AddSingleton<ILoggerProvider, EventViewerLoggingProvider>();
#endif
        return builder;
    }
}
