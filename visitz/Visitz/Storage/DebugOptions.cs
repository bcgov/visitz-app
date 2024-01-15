namespace Visitz.Storage
{
    public class DebugOptions
    {
        private static readonly string DryFireSubmitNotesKey = "DryFireSubmitNotes";
        private static readonly string DryFireSubmitNotesSimulateSuccessKey = "DryFireSubmitNotesSimulateSuccess";
        private static readonly string SkipLocalAuthKey = "SkipLocalAuth";
        private static readonly string ShowSafetyAssessmentKey = "ShowSafetyAssessment";

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

        public static bool DryFireSubmitNotes
        {
            get => Get(DryFireSubmitNotesKey, false);
            set => Set(DryFireSubmitNotesKey, value);
        }

        public static bool DryFireSubmitNotesSimulateSuccess
        {
            get => DryFireSubmitNotes && Get(DryFireSubmitNotesSimulateSuccessKey, false);
            set => Set(DryFireSubmitNotesSimulateSuccessKey, value);
        }

        public static bool SkipLocalAuth
        {
            get
            {
#if DEBUG
                return Get(SkipLocalAuthKey, false);
#else
                return false;
#endif
            }
            set => Set(SkipLocalAuthKey, value);
        }

        public static bool ShowSafetyAssessment
        {
            get => Get(ShowSafetyAssessmentKey, false);
            set => Set(ShowSafetyAssessmentKey, value);
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
