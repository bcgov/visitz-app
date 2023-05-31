namespace Visitz.Services
{
    public class DebugOptions
    {
        private static readonly string IdirOverrideKey = "IdirOverride";
        private static readonly string AlwaysExpireAccessTokenKey = "AlwaysExpireAccessTokenKey";
        private static readonly string AlwaysExpireRefreshTokenKey = "AlwaysExpireRefreshTokenKey";

        public static string IdirOverride
        {
            get => Preferences.Default.Get(IdirOverrideKey, "");
            set => Preferences.Default.Set(IdirOverrideKey, value.Trim());
        }

        public static bool AlwaysExpireAccessToken
        {
            get => Preferences.Default.Get(AlwaysExpireAccessTokenKey, false);
            set => Preferences.Default.Set(AlwaysExpireAccessTokenKey, value);
        }

        public static bool AlwaysExpireRefreshToken
        {
            get => Preferences.Default.Get(AlwaysExpireRefreshTokenKey, false);
            set => Preferences.Default.Set(AlwaysExpireRefreshTokenKey, value);
        }
    }
}
