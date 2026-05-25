using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc;
using Visitz.Services.Caseload;
using Visitz.Settings;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;
using Visitz.Views.Surveys;
using Visitz.Views.User;
using VisitzModel.Extensions;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;

namespace Visitz.Views.Debugging;

public partial class DebugOptionsViewModel : VisitzViewModel
{
    [ObservableProperty]
    public partial bool DryFireSubmitNotes { get; set; }

    [ObservableProperty]
    public partial bool DryFireSubmitNotesSimulateSuccess { get; set; }

    [ObservableProperty]
    public partial bool DryFirePostVisitService { get; set; }

    [ObservableProperty]
    public partial bool DryFirePostVisitServiceSimulateSuccess { get; set; }

    [ObservableProperty]
    public partial string AppId { get; set; }

    [ObservableProperty]
    public partial string DotnetVersion { get; set; }

    [ObservableProperty]
    public partial string ApiDomain { get; set; }

    [ObservableProperty]
    public partial string AuthenticationDomain { get; set; }

    [ObservableProperty]
    public partial bool BuildingInDebug { get; set; }

    [ObservableProperty]
    public partial bool SkipLocalAuth { get; set; }

    [ObservableProperty]
    public partial bool RequireAttachmentFileContent { get; set; }

    [ObservableProperty]
    public partial bool KeepSafetyAssessmentDraftOnPublish { get; set; }

    readonly LastUpdatedPrefs lastUpdatedPrefs = ServiceProvider.GetService<LastUpdatedPrefs>();

    [ObservableProperty]
    public partial DateTime CaseloadLastUpdated { get; set; }

    [ObservableProperty]
    public partial DateTime MaxDate { get; set; } = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public partial string MockPersonVisitsParentId { get; set; }

    [ObservableProperty]
    public partial bool AutoCaseloadRefreshDisabled { get; set; }

    [ObservableProperty]
    public partial double StaleSessionMinutes { get; set; }

    [ObservableProperty]
    public partial bool DisablePrivacyScrim { get; set; }

    [ObservableProperty]
    public partial bool WriteApiTimings { get; set; }

    [ObservableProperty]
    public partial double WindowHeight { get; set; }

    [ObservableProperty]
    public partial double WindowWidth { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        DryFireSubmitNotes = DebugOptions.Default.DryFireSubmitNotes;
        DryFireSubmitNotesSimulateSuccess = DebugOptions.Default.DryFireSubmitNotesSimulateSuccess;

        DryFirePostVisitService = DebugOptions.Default.DryFirePostVisitService;
        DryFirePostVisitServiceSimulateSuccess = DebugOptions.Default.DryFirePostVisitServiceSimulateSuccess;

        AppId = AppInfo.Current.PackageName;
        DotnetVersion = Environment.Version.ToString();

#if DEBUG
        BuildingInDebug = true;
#else
        BuildingInDebug = false;
#endif
        SkipLocalAuth = BuildingInDebug && DebugOptions.Default.SkipLocalAuth;

        RequireAttachmentFileContent = DebugOptions.Default.RequireAttachmentFileContent;
        KeepSafetyAssessmentDraftOnPublish = DebugOptions.Default.KeepSafetyAssessmentDraftOnPublish;

        var settings = new AppSettings();

        ApiDomain = settings.Api.ApiDomain;
        AuthenticationDomain = settings.Oidc.AuthenticationDomain;

        CaseloadLastUpdated = lastUpdatedPrefs.Get(GetCaseloadService.MakeId(), DateTimeExtensions.LocalNow);

        AutoCaseloadRefreshDisabled = DebugOptions.Default.AutoCaseloadRefreshDisabled;

        StaleSessionMinutes = DebugOptions.Default.StaleThresholdMinutes;

        DisablePrivacyScrim = DebugOptions.Default.DisablePrivacyScrim;

        WriteApiTimings = DebugOptions.Default.WriteApiTimings;

        if (Application.Current.Windows[0] is Window window)
        {
            WindowHeight = window.Height;
            WindowWidth = window.Width;
            window.SizeChanged += Window_SizeChanged;
        }
    }

    bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (Application.Current.Windows[0] is Window window)
                window.SizeChanged -= Window_SizeChanged;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void Window_SizeChanged(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            WindowHeight = window.Height;
            WindowWidth = window.Width;
        }
    }

    partial void OnDryFireSubmitNotesChanged(bool value)
    {
        DebugOptions.Default.DryFireSubmitNotes = value;
    }

    partial void OnDryFireSubmitNotesSimulateSuccessChanged(bool value)
    {
        DebugOptions.Default.DryFireSubmitNotesSimulateSuccess = value;
    }

    partial void OnDryFirePostVisitServiceChanged(bool value)
    {
        DebugOptions.Default.DryFirePostVisitService = value;
    }

    partial void OnDryFirePostVisitServiceSimulateSuccessChanged(bool value)
    {
        DebugOptions.Default.DryFirePostVisitServiceSimulateSuccess = value;
    }

    partial void OnSkipLocalAuthChanged(bool value)
    {
        DebugOptions.Default.SkipLocalAuth = value;
    }

    partial void OnRequireAttachmentFileContentChanged(bool value)
    {
        DebugOptions.Default.RequireAttachmentFileContent = value;
    }

    partial void OnKeepSafetyAssessmentDraftOnPublishChanged(bool value)
    {
        DebugOptions.Default.KeepSafetyAssessmentDraftOnPublish = value;
    }

    partial void OnAutoCaseloadRefreshDisabledChanged(bool value)
    {
        DebugOptions.Default.AutoCaseloadRefreshDisabled = value;
    }

    partial void OnStaleSessionMinutesChanged(double value)
    {
        DebugOptions.Default.StaleThresholdMinutes = value;
    }

    partial void OnDisablePrivacyScrimChanged(bool value)
    {
        DebugOptions.Default.DisablePrivacyScrim = value;
    }

    partial void OnWriteApiTimingsChanged(bool value)
    {
        DebugOptions.Default.WriteApiTimings = value;
    }

    [RelayCommand]
    public static void DeleteAccessToken()
    {
        if (DebugOptions.Default.Enabled)
            TokenHolder.DeleteAccessToken();
    }

    [RelayCommand]
    public static void DeleteRefreshToken()
    {
        if (DebugOptions.Default.Enabled)
            TokenHolder.DeleteRefreshToken();
    }

    [RelayCommand]
    public static async Task ClearRealmData()
    {
        await DebugOptions.Default.ClearRealmData();
    }

    [RelayCommand]
    public static async Task ClearSafetyAssessmentDraft()
    {
        await DebugOptions.Default.ClearSafetyAssessmentDraftsRealm();
    }

    [RelayCommand]
    public static async Task ClearAttachmentDraft()
    {
        await DebugOptions.Default.ClearAttachmentDraftsRealm();
    }

    [RelayCommand]
    public static async Task Logout()
    {
        if (DebugOptions.Default.Enabled)
            await OidcSession.LogoutAsync();
    }

    [RelayCommand]
    public static void ClearFeedbackSurveyPrefs()
    {
        new SurveyFeedbackTracker(Preferences.Default).ClearAll();
    }

    [RelayCommand]
    public void ApplyCaseloadLastUpdated()
    {
        lastUpdatedPrefs.Set(GetCaseloadService.MakeId(), CaseloadLastUpdated);
    }

    [RelayCommand]
    public static void OpenAppDataDirectory()
    {
        DebugOptions.Default.OpenAppDataDirectory();
    }

    [RelayCommand]
    public static void OpenCacheDirectory()
    {
        DebugOptions.Default.OpenCacheDirectory();
    }

    [RelayCommand]
    public static void ClearSecureStorage()
    {
        DebugOptions.Default.ClearSecureStorage();
    }

    [RelayCommand]
    public async Task LoadMockPersonVisits()
    {
        await DebugOptions.Default.LoadPersonVisitsMockData(MockPersonVisitsParentId);
    }

    [RelayCommand]
    public static async Task SetWarning()
    {
        await DebugOptions.Default.SetThreshold(VisitDaysThreshold.Warning);
    }

    [RelayCommand]
    public static async Task SetDanger()
    {
        await DebugOptions.Default.SetThreshold(VisitDaysThreshold.Danger);
    }

    [RelayCommand]
    public static async Task SetCritical()
    {
        await DebugOptions.Default.SetThreshold(VisitDaysThreshold.Critical);
    }

    [RelayCommand]
    public static Task ClearOfficeNames()
    {
        return DebugOptions.Default.ClearOfficeNames();
    }

    [RelayCommand]
    public static Task RemoveOneOffice()
    {
        return DebugOptions.Default.RemoveOneOffice();
    }

    [RelayCommand]
    public static async Task RunRecordCleanup()
    {
        await DebugOptions.Default.RunRecordCleanupService();
    }

    [RelayCommand]
    public static void RunAutoCaseloadRefreshService()
    {
        _ = DebugOptions.Default.RunAutoCaseloadRefreshService();
    }

    [RelayCommand]
    public static void ResetAutoCaseloadRefresh()
    {
        DebugOptions.Default.ResetAutoCaseloadRefresh();
    }

    [RelayCommand]
    public void ApplyNewDimensions()
    {
        DebugOptions.Default.WindowHeight = WindowHeight;
        DebugOptions.Default.WindowWidth = WindowWidth;

        // Values may have been clamped, so refresh values in the bindings
        WindowHeight = DebugOptions.Default.WindowHeight;
        WindowWidth = DebugOptions.Default.WindowWidth;
    }

    [RelayCommand]
    public void SwapWindowWidthAndHeight()
    {
        DebugOptions.Default.SwapWindowWidthAndHeight();

        // Values may have been clamped, so refresh values in the bindings
        WindowHeight = DebugOptions.Default.WindowHeight;
        WindowWidth = DebugOptions.Default.WindowWidth;
    }

    [RelayCommand]
    public void ApplyPhoneDimensions()
    {
        DebugOptions.Default.ApplyPhoneDimensions();

        // Values may have been clamped, so refresh values in the bindings
        WindowHeight = DebugOptions.Default.WindowHeight;
        WindowWidth = DebugOptions.Default.WindowWidth;
    }

    [RelayCommand]
    public void ApplyTabletDimensions()
    {
        DebugOptions.Default.ApplyTabletDimensions();

        // Values may have been clamped, so refresh values in the bindings
        WindowHeight = DebugOptions.Default.WindowHeight;
        WindowWidth = DebugOptions.Default.WindowWidth;
    }

    [RelayCommand]
    public void ApplyDefaultDesktopDimensions()
    {
        DebugOptions.Default.ApplyDefaultDesktopDimensions();

        // Values may have been clamped, so refresh values in the bindings
        WindowHeight = DebugOptions.Default.WindowHeight;
        WindowWidth = DebugOptions.Default.WindowWidth;
    }

    [RelayCommand]
    public static async Task ShowFeedbackPopupAsync()
    {
        await Navigator.Navigation.PushModalAsync(new FeedbackSurveyPage());
    }

    [RelayCommand]
    public static async Task OpenAppLockAsync()
    {
        await Navigator.Navigation.PushModalAsync(ServiceProvider.GetService<AppLockPage>());
    }

    [RelayCommand]
    public static async Task OpenSessionPageAsync()
    {
        await Navigator.Navigation.PushModalAsync(ServiceProvider.GetService<SessionPage>());
    }
}
