using MetroLog;
using MetroLog.MicrosoftExtensions;
using MetroLog.Targets;
using Microsoft.Extensions.Logging;
using MetroLogLevel = MetroLog.LogLevel;

namespace Visitz.VisitzConfig
{
    public static class VisitzLogging
    {
        public static MauiAppBuilder ConfigureVisitzLogging(this MauiAppBuilder builder)
        {
            // IStringLocalizer appears to be dependent on a logging service 
            builder.Services.AddLogging();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Logging
            .SetMinimumLevel((Microsoft.Extensions.Logging.LogLevel)MetroLogLevel.Trace)
            .AddStreamingFileLogger(
                options =>
                {
                    var logDirectory = Path.Combine(FileSystem.CacheDirectory, "MetroLogs");
                    options.FolderPath = logDirectory;
                }
            )
            .AddTraceLogger(
                options =>
                {
                    options.MinLevel = (Microsoft.Extensions.Logging.LogLevel?)MetroLogLevel.Trace;
                    options.MaxLevel = (Microsoft.Extensions.Logging.LogLevel?)MetroLogLevel.Fatal;
                }) // Will write to the Debug Output
            .AddConsoleLogger(
                options =>
                {
                    options.MinLevel = (Microsoft.Extensions.Logging.LogLevel?)MetroLogLevel.Trace;
                    options.MaxLevel = (Microsoft.Extensions.Logging.LogLevel?)MetroLogLevel.Fatal;
                }); // Will write to the Console Output

            ConfigureMetroLog(builder.Services);

            return builder;
        }
        private static void ConfigureMetroLog(IServiceCollection services)
        {
            var config = new LoggingConfiguration();

            config.AddTarget(
                MetroLogLevel.Trace,
                MetroLogLevel.Warn,
                new StreamingFileTarget(retainDays: 2));

            config.AddTarget(
                MetroLogLevel.Error,
                MetroLogLevel.Fatal,
                new StreamingFileTarget(retainDays: 30));

#if DEBUG            
            config.AddTarget(
                MetroLogLevel.Trace,
                MetroLogLevel.Fatal,
                new TraceTarget());
            
            config.AddTarget(
                MetroLogLevel.Trace, 
                MetroLogLevel.Fatal, 
                new ConsoleTarget());
#endif
        
            MetroLog.LoggerFactory.Initialize(config);
        }
    }
}
