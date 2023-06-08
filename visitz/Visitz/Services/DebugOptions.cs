using Realms;
using Visitz.Storage;

namespace Visitz.Services
{
    public class DebugOptions
    {
        private static readonly string IdirOverrideKey = "IdirOverride";
        private static readonly string AlwaysExpireAccessTokenKey = "AlwaysExpireAccessTokenKey";
        private static readonly string AlwaysExpireRefreshTokenKey = "AlwaysExpireRefreshTokenKey";

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

        public static bool AlwaysExpireAccessToken
        {
            get => Get(AlwaysExpireAccessTokenKey, false);
            set => Set(AlwaysExpireAccessTokenKey, value);
        }

        public static bool AlwaysExpireRefreshToken
        {
            get => Get(AlwaysExpireRefreshTokenKey, false);
            set => Set(AlwaysExpireRefreshTokenKey, value);
        }

        public static async Task ClearRealmData()
        {
            if (Enabled)
            {
                using var realm = await VisitzRealm.GetAsync();
                await realm.WriteAsync(realm.RemoveAll);
            }
        }

        public static void DeleteEncryptionKey()
        {
            if (Enabled)
                VisitzRealm.DeleteRealmKey();
        }
    }
}
