using Visitz.Settings;
using Visitz.Storage;

namespace Visitz.VisitzConfig
{
    public static class VisitzDebugOptions
    {
        public static void ConfigureVisitzDebugOptions()
        {
            var enableDebug = new AppSettings().Debug.EnableDebugSettings;
            Preferences.Default.Set(DebugOptions.EnableOptionsKey, enableDebug);
        }
    }
}
