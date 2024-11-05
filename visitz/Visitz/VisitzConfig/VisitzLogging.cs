using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;
using MetroLogLevel = MetroLog.LogLevel;
using Visitz.Storage;

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
            .AddTraceLogger(
                options =>
                {
                    options.MaxLevel = (MicrosoftLogLevel?)MetroLogLevel.Fatal;
                })
            .AddConsoleLogger(
                options =>
                {
                    options.MaxLevel = (MicrosoftLogLevel?)MetroLogLevel.Fatal;
                });

            builder.Logging.Services.AddSingleton<ILoggerProvider, RealmAsyncTarget>();
            return builder;
        }
    }
}
