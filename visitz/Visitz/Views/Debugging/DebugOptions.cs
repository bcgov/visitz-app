using System.Text;
using Oidc;
using Visitz.Services;
using Visitz.Services.Caseload;
using Visitz.Storage;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;
#if WINDOWS
using Windows.Storage;
using Microsoft.Maui.Controls.Internals;
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
    private static readonly string AutoCaseloadRefreshDisabledKey = "AutoCaseloadRefreshDisabled";
    private static readonly string StaleThresholdMinutesKey = "StaleThresholdMinutes";
    private static readonly string DisablePrivacyScrimKey = "EnableObscuringScrim";
    private static readonly string WriteApiTimingsKey = "WriteApiTimings";
    private static readonly string WindowHeightKey = "WindowHeight";
    private static readonly string WindowWidthKey = "WindowWidth";

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
        return Enabled ? Preferences.Default.Get(key, defaultValue) : defaultValue;
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

    public static bool AutoCaseloadRefreshDisabled
    {
        get => Get(AutoCaseloadRefreshDisabledKey, false);
        set => Set(AutoCaseloadRefreshDisabledKey, value);
    }

    public static double StaleThresholdMinutes
    {
        get => Get(StaleThresholdMinutesKey, OidcSession.StaleThresholdMinutes);
        set => Set(StaleThresholdMinutesKey, value > 0.0d ? value : OidcSession.StaleThresholdMinutes);
    }

    public static bool DisablePrivacyScrim
    {
        get => Get(DisablePrivacyScrimKey, false);
        set => Set(DisablePrivacyScrimKey, value);
    }

    public static bool WriteApiTimings
    {
        get => Get(WriteApiTimingsKey, false);
        set => Set(WriteApiTimingsKey, value);
    }

    public static double WindowHeight
    {
        get => Get(WindowHeightKey, Application.Current.Windows[0].Height);
        set
        {
            double val = Math.Clamp(value, 100, DeviceDisplay.MainDisplayInfo.Height);
            Set(WindowHeightKey, val);
            Application.Current.Windows[0].Height = val;
        }
    }

    public static double WindowWidth
    {
        get => Get(WindowWidthKey, Application.Current.Windows[0].Width);
        set
        {
            double val = Math.Clamp(value, 100, DeviceDisplay.MainDisplayInfo.Width);
            Set(WindowWidthKey, val);
            Application.Current.Windows[0].Width = val;
        }
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
        string[] files = Directory.GetFileSystemEntries(
            path,
            "**",
            new EnumerationOptions() { RecurseSubdirectories = true }
        );

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
        return Enabled ? ListFilesRecursively(path) : string.Empty;
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

        var latestVisits = icmData.All<PersonVisit>();

        var extraDay = threshold == VisitDaysThreshold.Critical ? 1 : 0;
        var targetDueDate = DateTimeOffset.Now.Date.AddDays((int)threshold - extraDay);

        var targetDateOfVisit = targetDueDate.AddDays(-(int)VisitDaysThreshold.Info);

        await icmData.WriteAsync(() =>
        {
            foreach (var visit in latestVisits)
                visit.DateOfVisit = targetDateOfVisit;
        });
    }

    public static async Task ClearOfficeNames()
    {
        if (!Enabled)
            return;

        var info = await OidcSession.GetInfoAsync();
        info.OfficeNames = [];
    }

    public static async Task RemoveOneOffice()
    {
        if (!Enabled)
            return;

        var info = await OidcSession.GetInfoAsync();
        info.OfficeNames = info.OfficeNames.Skip(1).ToHashSet();
    }

    public static async Task RunRecordCleanupService()
    {
        if (!Enabled)
            return;

        await ServiceProvider.GetService<ServiceHandler>().TryRunServiceAsync(RecordCleanupService.MakeStartMessage());
    }

    public static async Task RunAutoCaseloadRefreshService()
    {
        if (!Enabled)
            return;

        var handler = ServiceProvider.GetService<ServiceHandler>();
        await handler.TryRunServiceAsync(AutoRefreshService.MakeStartMessage());
    }

    public static void ResetAutoCaseloadRefresh()
    {
        if (!Enabled)
            return;

        LastUpdatedPrefs prefs = ServiceProvider.GetService<LastUpdatedPrefs>();
        prefs.Set(AutoRefreshService.CooldownTimestampUtc, DateTime.MinValue);
    }

    public static void SwapWindowWidthAndHeight()
    {
        if (!Enabled)
            return;

        Window window = Application.Current.Windows[0];
        (window.Width, window.Height) = (window.Height, window.Width);

        WindowWidth = window.Width;
        WindowHeight = window.Height;
    }

    public static void ApplyPhoneDimensions()
    {
        if (!Enabled)
            return;

        // iPhone SE dims manually collected from runtime
        WindowHeight = 667;
        WindowWidth = 375;
    }

    public static void ApplyTabletDimensions()
    {
        if (!Enabled)
            return;

        // iPad Air 4 dims manually collected from runtime
        WindowHeight = 1180;
        WindowWidth = 820;
    }

    public static void ApplyDefaultDesktopDimensions()
    {
        if (!Enabled)
            return;

        WindowHeight = VisitzWindow.InitialHeight;
        WindowWidth = VisitzWindow.InitialHeight * VisitzWindow.InitialWidthRatio;
    }
}
