namespace Visitz.Storage
{
    public class DebugOptions
    {
        private static readonly string IdirOverrideKey = "IdirOverride";
        private static readonly string ShowNoteItemViewDebugInfoKey = "ShowNoteItemViewDebugInfo";

        public static readonly string EnableOptionsKey = "EnableDebugOptions";

        public static bool Enabled => Preferences.Default.Get(EnableOptionsKey, false);

        private static T Get<T>(string key, T defaultValue)
        {
            return Enabled
                ? Preferences.Default.Get(key, defaultValue)
                : defaultValue;
        }

        private static void Set<T>(string key, T value)
        {
            if (Enabled)
                Preferences.Default.Set(key, value);
        }

        public static string IdirOverride
        {
            get => Get(IdirOverrideKey, "");
            set => Set(IdirOverrideKey, value.Trim());
        }

        public static bool ShowNoteItemViewDebugInfo
        {
            get => Get(ShowNoteItemViewDebugInfoKey, false);
            set => Set(ShowNoteItemViewDebugInfoKey, value);
        }

        public static async Task ClearRealmData()
        {
            if (Enabled)
                await VisitzRealm.ClearIcmDataRealm();
        }

        public static void DeleteEncryptionKey()
        {
            if (Enabled)
                VisitzRealm.DeleteRealmKey(VisitzRealm.IcmDataCopiesPath);
        }
    }
}
