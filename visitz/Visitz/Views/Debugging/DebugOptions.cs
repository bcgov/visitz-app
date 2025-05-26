using System.Text;
using Visitz.Storage;
using VisitzModel.Models.InPersonVisits;


#if WINDOWS
using Windows.Storage;
#endif

#if WINDOWS || MACCATALYST
using System.Diagnostics;
#endif

namespace Visitz.Views.Debugging;

public class DebugOptions
{
    private static readonly string DryFireSubmitNotesKey = "DryFireSubmitNotes";
    private static readonly string DryFireSubmitNotesSimulateSuccessKey = "DryFireSubmitNotesSimulateSuccess";
    private static readonly string DryFirePostVisitServiceKey = "DryFirePostVisitService";
    private static readonly string DryFirePostVisitServiceSimulateSuccessKey = "DryFirePostVisitServiceSimulateSuccess";
    private static readonly string SkipLocalAuthKey = "SkipLocalAuth";
    private static readonly string ShouldExpectFileContentKey = "ShouldExpectFileContent";
    private static readonly string KeepSafetyAssessmentDraftOnPublishKey = "KeepSafetyAssessmentDraftOnPublish";

    public static readonly string EnableOptionsKey = "EnableDebugOptions";

    public static bool Enabled => Preferences.Default.Get(EnableOptionsKey, false);

    public static void TryStartShakeDetector(Action actionOnShake)
    {
        if (!Enabled)
            return;

        if (Accelerometer.Default.IsSupported)
        {
            Accelerometer.Default.ShakeDetected += (sender, args) => actionOnShake();
            Accelerometer.Default.Start(SensorSpeed.Game);
        }
        else
            Console.WriteLine("Accelerometer not supported");
    }

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

    public static bool DryFirePostVisitService
    {
        get => Get(DryFirePostVisitServiceKey, false);
        set => Set(DryFirePostVisitServiceKey, value);
    }

    public static bool DryFirePostVisitServiceSimulateSuccess
    {
        get => DryFirePostVisitService && Get(DryFirePostVisitServiceSimulateSuccessKey, false);
        set => Set(DryFirePostVisitServiceSimulateSuccessKey, value);
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

    public static bool RequireAttachmentFileContent
    {
        get => Get(ShouldExpectFileContentKey, true);
        set => Set(ShouldExpectFileContentKey, value);
    }

    public static bool KeepSafetyAssessmentDraftOnPublish
    {
        get => Get(KeepSafetyAssessmentDraftOnPublishKey, false);
        set => Set(KeepSafetyAssessmentDraftOnPublishKey, value);
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

    public static async Task ClearAttachmentDraftsRealm()
    {
        if (Enabled)
        {
            await (await VisitzRealms.GetAttachmentDraftsAsync()).ClearAllData();

            string attachmentsPath = Path.Join(FileSystem.AppDataDirectory, "Attachments");

            if (Directory.Exists(attachmentsPath))
                Directory.Delete(attachmentsPath, true);
        }
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

    static string ListFilesRecursively(string path)
    {
        string[] files = Directory.GetFileSystemEntries(path, "**", new EnumerationOptions()
        {
            RecurseSubdirectories = true,
        });

        StringBuilder filesOut = new();

        filesOut.AppendLine($"Files in '{path}':");
        foreach (var file in files)
            filesOut.AppendLine(file);
        filesOut.AppendLine("-------");

        return filesOut.ToString();
    }

    public static string ListDocumentsFiles()
    {
#if WINDOWS
        string path = FileSystem.AppDataDirectory;
#else
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
        return Enabled
            ? ListFilesRecursively(path)
            : string.Empty;
    }

    public static void ClearSecureStorage()
    {
        if (Enabled)
            SecureStorage.Default.RemoveAll();
    }

#if WINDOWS
    public static Dictionary<string, string> GetAllValuesFromLocalSettings()
    {
        if (!Enabled)
            return default;

        return GetAllValuesFrom(ApplicationData.Current.LocalSettings);
    }
#endif

#if WINDOWS
    static Dictionary<string, string> GetAllValuesFrom(ApplicationDataContainer container)
    {
        Dictionary<string, string> result = [];

        foreach (var key in container.Values.Keys)
            result[$"({container.Name}), Key '{key}'"] = container.Values[key]?.ToString();

        foreach (var subContainer in container.Containers)
            foreach (var valuesResult in GetAllValuesFrom(subContainer.Value))
                if (result.ContainsKey(valuesResult.Key))
                    result[subContainer.Key + " > " + valuesResult.Key + "+"] = valuesResult.Value;
                else
                    result[subContainer.Key + " > " + valuesResult.Key] = valuesResult.Value;

        return result;
    }
#endif

    public static async Task LoadPersonVisitsMockData(string parentId)
    {
        if (!Enabled)
            return;

        using var icmData = await VisitzRealms.GetIcmDataRealmAsync();

        await icmData.WriteAsync(() => icmData.Add(SimpleMockData.MockPersonVisits(parentId), update: true));
    }

    public static async Task SetThreshold(VisitDaysThreshold threshold)
    {
        if (!Enabled)
            return;

        using var icmData = await VisitzRealms.GetIcmDataRealmAsync();

        var latestVisits = PersonVisit.GetAllByType(icmData)
            .AsEnumerable()
            .GroupBy(item => item.ParentId)
            .Select(group => group
            .OrderByDescending(item => item.DateOfVisit)
            .FirstOrDefault())
            .Where(item => item != null)
            .ToList();

        var targetDueDate = DateTimeOffset.Now.Date.AddDays((int)threshold);
        var targetDateOfVisit = targetDueDate.AddDays(-(int)VisitDaysThreshold.Info);

        await icmData.WriteAsync(() =>
        {
            foreach (var visit in latestVisits)
                visit.DateOfVisit = targetDateOfVisit;
        });
    }
}
