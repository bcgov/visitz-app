using Microsoft.Extensions.Logging;

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

            return builder;
        }
    }
}
