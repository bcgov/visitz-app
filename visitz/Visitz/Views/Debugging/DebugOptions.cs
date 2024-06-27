using System.Diagnostics;
using System.Text.Json;
using Visitz.Storage;
using VisitzModel.Models;

namespace Visitz.Views.Debugging
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
                await (await VisitzRealms.GetIcmDataAsync()).ClearAllData();
        }

        public static async Task ClearSafetyAssessmentDraftsRealm()
        {
            if (Enabled)
                await (await VisitzRealms.GetSafetyAssessmentDraftAsync()).ClearAllData();
        }

        public static async Task Load620bTestingRecords()
        {
            await using var json = await FileSystem.OpenAppPackageFileAsync(Path.Join("MockIcmData", "620b.json"));

            var opts = new JsonSerializerOptions() 
            {
                PropertyNameCaseInsensitive = true,
                PreferredObjectCreationHandling = System.Text.Json.Serialization.JsonObjectCreationHandling.Populate
            };

            var caseload = await JsonSerializer.DeserializeAsync<IEnumerable<CaseloadItem>>(json, options: opts);
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();

            await realm.WriteAsync(() => realm.Add(caseload, update: true));
        }

		public static void OpenAppDataDirectory()
		{
#if WINDOWS || MACCATALYST
			if (Enabled)
				Process.Start("explorer.exe", FileSystem.AppDataDirectory);
#endif
		}

		public static void OpenCacheDirectory()
		{
#if WINDOWS || MACCATALYST
			if (Enabled)
				Process.Start("explorer.exe", FileSystem.CacheDirectory);
#endif
		}
	}
}
