using Microsoft.Extensions.Logging;

namespace hestia.HestiaConfig
{
    public static class HestiaLogging
    {
        public static MauiAppBuilder ConfigureHestiaLogging(this MauiAppBuilder builder)
        {
            // IStringLocalizer appears to be dependent on a logging service 
            builder.Services.AddLogging();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder;
        }
    }
}
