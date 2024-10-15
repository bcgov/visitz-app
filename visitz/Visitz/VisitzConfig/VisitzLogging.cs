using MetroLog;
using MetroLog.MicrosoftExtensions;
using MetroLog.Targets;
using Microsoft.Extensions.Logging;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;
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
            .SetMinimumLevel((MicrosoftLogLevel)MetroLogLevel.Trace)
            .AddStreamingFileLogger(
                options =>
                {
                    var logDirectory = Path.Combine(FileSystem.CacheDirectory, "MetroLogs");
                    options.FolderPath = logDirectory;
                    options.MaxLevel = (MicrosoftLogLevel?)MetroLogLevel.Fatal;
                    options.RetainDays = 30;
                }
            )
            .AddTraceLogger(
                options =>
                {
                    options.MaxLevel = (MicrosoftLogLevel?)MetroLogLevel.Fatal;
                }) // Will write to the Debug Output
            .AddConsoleLogger(
                options =>
                {
                    options.MaxLevel = (Microsoft.Extensions.Logging.LogLevel?)MetroLogLevel.Fatal;
                }); // Will write to the Console Output

            return builder;
        }
    }
}
