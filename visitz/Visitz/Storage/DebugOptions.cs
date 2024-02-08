using System.Text.Json;
using VisitzModel.Models;

namespace Visitz.Storage
{
    public class DebugOptions
    {
        private static readonly string DryFireSubmitNotesKey = "DryFireSubmitNotes";
        private static readonly string DryFireSubmitNotesSimulateSuccessKey = "DryFireSubmitNotesSimulateSuccess";
        private static readonly string SkipLocalAuthKey = "SkipLocalAuth";

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

        public static async Task ClearRealmData()
        {
            if (Enabled)
                await VisitzRealm.ClearIcmDataRealm();
        }

        public static async Task ClearSafetyAssessmentDraftsRealm()
        {
            if (Enabled)
                await VisitzRealm.ClearSafetyAssessmentDraftRealm();
        }

        public static void DeleteEncryptionKey()
        {
            if (Enabled)
                VisitzRealm.DeleteRealmKey(VisitzRealm.IcmDataCopiesPath);
        }

        public static async Task Load620bTestingRecords()
        {
            using var json = await FileSystem.OpenAppPackageFileAsync(Path.Join("MockIcmData", "620b.json"));

            var opts = new JsonSerializerOptions() 
            {
                PropertyNameCaseInsensitive = true,
                PreferredObjectCreationHandling = System.Text.Json.Serialization.JsonObjectCreationHandling.Populate
            };

            var caseload = await JsonSerializer.DeserializeAsync<IEnumerable<CaseloadItem>>(json, options: opts);
            using var realm = await VisitzRealm.GetIcmDataAsync();

            await realm.WriteAsync(() => realm.Add(caseload, update: true));
        }
    }
}
