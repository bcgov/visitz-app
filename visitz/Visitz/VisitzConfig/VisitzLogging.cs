using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using Visitz.Storage;
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

        LogLevel minimumLogLevel = LogLevel.Error;

#if DEBUG
        minimumLogLevel = LogLevel.Debug;

        builder
            .Logging.AddDebug()
            .AddConsoleLogger(options =>
            {
                options.MinLevel = minimumLogLevel;
                options.MaxLevel = LogLevel.Critical;
            });
#endif

        builder
            .Logging.SetMinimumLevel(minimumLogLevel)
            .AddRealmLogger(options =>
            {
                options.MinimumLogLevel = minimumLogLevel;
                options.MaximumLogLevel = LogLevel.Critical;
            });

#if WINDOWS
        builder.Logging.Services.AddSingleton<ILoggerProvider, EventViewerLoggingProvider>();
#endif
        return builder;
    }
}
