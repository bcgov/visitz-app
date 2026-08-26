using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.Extensions;
using Visitz.Services;
using Visitz.Services.AppLogs;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.AppLogs;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
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

public partial class DebugOptions(IPreferences preferences) : ObservableObject
{
    const string DryFireSubmitNotesKey = "DryFireSubmitNotes";
    const string DryFireSubmitNotesSimulateSuccessKey = "DryFireSubmitNotesSimulateSuccess";
    const string DryFirePostVisitServiceKey = "DryFirePostVisitService";
    const string DryFirePostVisitServiceSimulateSuccessKey = "DryFirePostVisitServiceSimulateSuccess";
    const string SkipLocalAuthKey = "SkipLocalAuth";
    const string ShouldExpectFileContentKey = "ShouldExpectFileContent";
    const string KeepSafetyAssessmentDraftOnPublishKey = "KeepSafetyAssessmentDraftOnPublish";
    const string AutoCaseloadRefreshDisabledKey = "AutoCaseloadRefreshDisabled";
    const string StaleThresholdMinutesKey = "StaleThresholdMinutes";
    const string DisablePrivacyScrimKey = "EnableObscuringScrim";
    const string WriteApiTimingsKey = "WriteApiTimings";
    const string WindowHeightKey = "WindowHeight";
    const string WindowWidthKey = "WindowWidth";
    const string ShowBottomNavOnWindowsKey = "ShowBottomNavOnWindows";
    const string RunAppLogsServiceInDebugKey = "RunAppLogsServiceInDebug";
    const string DryFireSendAppLogsKey = "DryFireSendAppLogs";
    const string KeepLogsAfterSendingKey = "DeleteLogsAfterSending";

    public static readonly string EnableOptionsKey = "EnableDebugOptions";

    public static readonly DebugOptions Default = new(Preferences.Default);

    IPreferences AppPreferences { get; set; } = preferences;

    public bool Enabled => AppPreferences.Get(EnableOptionsKey, false);

    public void TryStartShakeDetector(Action actionOnShake)
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

    T Get<T>(string key, T defaultValue)
    {
        return Enabled ? AppPreferences.Get(key, defaultValue) : defaultValue;
    }

    void Set<T>(string key, T value, [CallerMemberName] string caller = "")
    {
        if (!Enabled)
            return;

        OnPropertyChanging(caller);
        AppPreferences.Set(key, value);
        OnPropertyChanged(caller);
    }

    public bool DryFireSubmitNotes
    {
        get => Get(DryFireSubmitNotesKey, false);
        set => Set(DryFireSubmitNotesKey, value);
    }

    public bool DryFireSubmitNotesSimulateSuccess
    {
        get => DryFireSubmitNotes && Get(DryFireSubmitNotesSimulateSuccessKey, false);
        set => Set(DryFireSubmitNotesSimulateSuccessKey, value);
    }

    public bool DryFirePostVisitService
    {
        get => Get(DryFirePostVisitServiceKey, false);
        set => Set(DryFirePostVisitServiceKey, value);
    }

    public bool DryFirePostVisitServiceSimulateSuccess
    {
        get => DryFirePostVisitService && Get(DryFirePostVisitServiceSimulateSuccessKey, false);
        set => Set(DryFirePostVisitServiceSimulateSuccessKey, value);
    }

    public bool SkipLocalAuth
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

    public bool RequireAttachmentFileContent
    {
        get => Get(ShouldExpectFileContentKey, true);
        set => Set(ShouldExpectFileContentKey, value);
    }

    public bool KeepSafetyAssessmentDraftOnPublish
    {
        get => Get(KeepSafetyAssessmentDraftOnPublishKey, false);
        set => Set(KeepSafetyAssessmentDraftOnPublishKey, value);
    }

    public bool AutoCaseloadRefreshDisabled
    {
        get => Get(AutoCaseloadRefreshDisabledKey, false);
        set => Set(AutoCaseloadRefreshDisabledKey, value);
    }

    public double StaleThresholdMinutes
    {
        get => Get(StaleThresholdMinutesKey, OidcSession.StaleThresholdMinutes);
        set => Set(StaleThresholdMinutesKey, value > 0.0d ? value : OidcSession.StaleThresholdMinutes);
    }

    public bool DisablePrivacyScrim
    {
        get => Get(DisablePrivacyScrimKey, false);
        set => Set(DisablePrivacyScrimKey, value);
    }

    public bool WriteApiTimings
    {
        get => Get(WriteApiTimingsKey, false);
        set => Set(WriteApiTimingsKey, value);
    }

    public double WindowHeight
    {
        get => Get(WindowHeightKey, Application.Current?.Windows[0].Height ?? 0.0d);
        set
        {
            double val = Math.Clamp(value, 100, DeviceDisplay.MainDisplayInfo.Height * 2);
            Set(WindowHeightKey, val);
            Application.Current?.Windows[0].Height = val;
        }
    }

    public double WindowWidth
    {
        get => Get(WindowWidthKey, Application.Current?.Windows[0].Width ?? 0.0d);
        set
        {
            double val = Math.Clamp(value, 100, DeviceDisplay.MainDisplayInfo.Width * 2);
            Set(WindowWidthKey, val);
            Application.Current?.Windows[0].Width = val;
        }
    }

    public bool ShowBottomNavOnWindows
    {
        get => Get(ShowBottomNavOnWindowsKey, false);
        set => Set(ShowBottomNavOnWindowsKey, value);
    }

    public bool RunAppLogsServiceInDebug
    {
        get => Get(RunAppLogsServiceInDebugKey, false);
        set => Set(RunAppLogsServiceInDebugKey, value);
    }

    public bool DryFireSendAppLogs
    {
        get => Get(DryFireSendAppLogsKey, false);
        set => Set(DryFireSendAppLogsKey, value);
    }

    public bool KeepLogsAfterSending
    {
        get => Get(KeepLogsAfterSendingKey, false);
        set => Set(KeepLogsAfterSendingKey, value);
    }

    [RelayCommand]
    public async Task ClearRealmData()
    {
        if (Enabled)
            await (await VisitzRealms.GetIcmDataAsync()).ClearAllData();
    }

    [RelayCommand]
    public async Task ClearSafetyAssessmentDraftsRealm()
    {
        if (Enabled)
            await (await VisitzRealms.GetSafetyAssessmentDraftAsync()).ClearAllData();
    }

    [RelayCommand]
    public async Task ClearAttachmentDraftsRealm()
    {
        if (Enabled)
        {
            await (await VisitzRealms.GetAttachmentDraftsAsync()).ClearAllData();

            string attachmentsPath = Path.Join(FileSystem.AppDataDirectory, "Attachments");

            if (Directory.Exists(attachmentsPath))
                Directory.Delete(attachmentsPath, true);
        }
    }

    [RelayCommand]
    public void OpenAppDataDirectory()
    {
#if WINDOWS || MACCATALYST
        if (Enabled)
            Process.Start("explorer.exe", FileSystem.AppDataDirectory);
#endif
    }

    [RelayCommand]
    public void OpenCacheDirectory()
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

    public string ListDocumentsFiles()
    {
#if WINDOWS
        string path = FileSystem.AppDataDirectory;
#else
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
        return Enabled ? ListFilesRecursively(path) : string.Empty;
    }

    [RelayCommand]
    public void ClearSecureStorage()
    {
        if (Enabled)
            SecureStorage.Default.RemoveAll();
    }

#if WINDOWS
    public Dictionary<string, string> GetAllValuesFromLocalSettings()
    {
        if (!Enabled)
            return [];

        return GetAllValuesFrom(ApplicationData.Current.LocalSettings);
    }
#endif

#if WINDOWS
    Dictionary<string, string> GetAllValuesFrom(ApplicationDataContainer container)
    {
        Dictionary<string, string> result = [];

        foreach (var key in container.Values.Keys)
            result[$"({container.Name}), Key '{key}'"] = container.Values[key]?.ToString() ?? string.Empty;

        foreach (var subContainer in container.Containers)
        foreach (var valuesResult in GetAllValuesFrom(subContainer.Value))
            if (result.ContainsKey(valuesResult.Key))
                result[subContainer.Key + " > " + valuesResult.Key + "+"] = valuesResult.Value;
            else
                result[subContainer.Key + " > " + valuesResult.Key] = valuesResult.Value;

        return result;
    }
#endif

    [RelayCommand]
    public async Task LoadPersonVisitsMockData(string parentId)
    {
        if (!Enabled)
            return;

        using var icmData = await VisitzRealms.GetIcmDataRealmAsync();

        await icmData.WriteAsync(() => icmData.Add(SimpleMockData.MockPersonVisits(parentId), update: true));
    }

    [RelayCommand]
    public async Task SetThreshold(VisitDaysThreshold threshold)
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

    [RelayCommand]
    public async Task ClearOfficeNames()
    {
        if (!Enabled)
            return;

        var info = await OidcSession.GetInfoAsync();
        info.OfficeNames = [];
    }

    [RelayCommand]
    public async Task RemoveOneOffice()
    {
        if (!Enabled)
            return;

        var info = await OidcSession.GetInfoAsync();
        info.OfficeNames = info.OfficeNames.Skip(1).ToHashSet();
    }

    [RelayCommand]
    public async Task RunRecordCleanupService()
    {
        if (!Enabled)
            return;

        await ServiceProvider.GetService<ServiceHandler>().TryRunServiceAsync(RecordCleanupService.MakeStartMessage());
    }

    [RelayCommand]
    public async Task RunAutoCaseloadRefreshService()
    {
        if (!Enabled)
            return;

        var handler = ServiceProvider.GetService<ServiceHandler>();
        await handler.TryRunServiceAsync(AutoRefreshService.MakeStartMessage());
    }

    [RelayCommand]
    public void ResetAutoCaseloadRefresh()
    {
        if (!Enabled)
            return;

        LastUpdatedPrefs prefs = ServiceProvider.GetService<LastUpdatedPrefs>();
        prefs.Set(AutoRefreshService.CooldownTimestampUtc, DateTime.MinValue);
    }

    [RelayCommand]
    public void SwapWindowWidthAndHeight()
    {
        if (!Enabled)
            return;

        if (Application.Current?.Windows[0] is Window window)
        {
            (window.Width, window.Height) = (window.Height, window.Width);

            WindowWidth = window.Width;
            WindowHeight = window.Height;
        }
    }

    [RelayCommand]
    public void ApplyPhoneDimensions()
    {
        if (!Enabled)
            return;

        // iPhone SE dims manually collected from runtime
        WindowHeight = 667;
        WindowWidth = 375;
    }

    [RelayCommand]
    public void ApplyTabletDimensions()
    {
        if (!Enabled)
            return;

        // iPad Air 4 dims manually collected from runtime
        WindowHeight = 820;
        WindowWidth = 1180;
    }

    [RelayCommand]
    public void ApplyDefaultDesktopDimensions()
    {
        if (!Enabled)
            return;

        WindowHeight = VisitzWindow.InitialHeight;
        WindowWidth = VisitzWindow.InitialHeight * VisitzWindow.InitialWidthRatio;
    }

    [RelayCommand]
    public static void ShowSnackbar(string? text = null)
    {
        text ??= $"An example string of text written for this snackbar to display for {int.MaxValue} seconds.";
        SnackbarHandler.ShowTextWithDetails(
            text,
            "testing title",
            "testing message",
            TimeSpan.FromSeconds(int.MaxValue)
        );
    }

    [RelayCommand]
    public static void RunSendAppLogsService()
    {
        WeakReferenceMessenger.Default.Send(SendAppLogsService.MakeStartMessage());
    }

    [RelayCommand]
    public static async Task OpenLogsWindow()
    {
        await Navigator.Navigation.PushModalAsync(new AppLogsList(new()), ViewModalSize.Fullscreen);
    }

    [RelayCommand]
    public static void WriteTestingLogs()
    {
        var logger = ServiceProvider.GetService<ILogger<DebugOptions>>();
        logger.LogTrace("Trace log");
        logger.LogDebug("Debug log");
        logger.LogInformation("Information log");
        logger.LogWarning("Warning log");
        logger.LogError("Error log");

        try
        {
            throw new Exception("log exception");
        }
        catch (Exception ex)
        {
            logger.LogException(ex);
        }

        try
        {
            throw new Exception("log exception");
        }
        catch (Exception ex)
        {
            logger.LogException(ex, "log exception with custom message");
        }

        logger.LogCritical("Critical log");
    }
}
