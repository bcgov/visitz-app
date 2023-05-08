using Microsoft.Extensions.Logging;

namespace hestia.HestiaConfig
{
    public class HestiaLogging
    {
        public static MauiAppBuilder ConfigureHestiaLogging(MauiAppBuilder builder)
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
