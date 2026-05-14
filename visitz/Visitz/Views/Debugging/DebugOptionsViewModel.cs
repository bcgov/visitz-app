using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc;
using Visitz.Services.Caseload;
using Visitz.Settings;
using Visitz.Views.BaseClasses;
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

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        DryFireSubmitNotes = DebugOptions.DryFireSubmitNotes;
        DryFireSubmitNotesSimulateSuccess = DebugOptions.DryFireSubmitNotesSimulateSuccess;

        DryFirePostVisitService = DebugOptions.DryFirePostVisitService;
        DryFirePostVisitServiceSimulateSuccess = DebugOptions.DryFirePostVisitServiceSimulateSuccess;

        AppId = AppInfo.Current.PackageName;
        DotnetVersion = Environment.Version.ToString();

#if DEBUG
        BuildingInDebug = true;
#else
        BuildingInDebug = false;
#endif
        SkipLocalAuth = BuildingInDebug && DebugOptions.SkipLocalAuth;

        RequireAttachmentFileContent = DebugOptions.RequireAttachmentFileContent;
        KeepSafetyAssessmentDraftOnPublish = DebugOptions.KeepSafetyAssessmentDraftOnPublish;

        var settings = new AppSettings();

        ApiDomain = settings.Api.ApiDomain;
        AuthenticationDomain = settings.Oidc.AuthenticationDomain;

        CaseloadLastUpdated = lastUpdatedPrefs.Get(GetCaseloadService.MakeId(), DateTimeExtensions.LocalNow);

        AutoCaseloadRefreshDisabled = DebugOptions.AutoCaseloadRefreshDisabled;

        StaleSessionMinutes = DebugOptions.StaleThresholdMinutes;

        DisablePrivacyScrim = DebugOptions.DisablePrivacyScrim;
    }

    partial void OnDryFireSubmitNotesChanged(bool value)
    {
        DebugOptions.DryFireSubmitNotes = value;
    }

    partial void OnDryFireSubmitNotesSimulateSuccessChanged(bool value)
    {
        DebugOptions.DryFireSubmitNotesSimulateSuccess = value;
    }

    partial void OnDryFirePostVisitServiceChanged(bool value)
    {
        DebugOptions.DryFirePostVisitService = value;
    }

    partial void OnDryFirePostVisitServiceSimulateSuccessChanged(bool value)
    {
        DebugOptions.DryFirePostVisitServiceSimulateSuccess = value;
    }

    partial void OnSkipLocalAuthChanged(bool value)
    {
        DebugOptions.SkipLocalAuth = value;
    }

    partial void OnRequireAttachmentFileContentChanged(bool value)
    {
        DebugOptions.RequireAttachmentFileContent = value;
    }

    partial void OnKeepSafetyAssessmentDraftOnPublishChanged(bool value)
    {
        DebugOptions.KeepSafetyAssessmentDraftOnPublish = value;
    }

    partial void OnAutoCaseloadRefreshDisabledChanged(bool value)
    {
        DebugOptions.AutoCaseloadRefreshDisabled = value;
    }

    partial void OnStaleSessionMinutesChanged(double value)
    {
        DebugOptions.StaleThresholdMinutes = value;
    }

    partial void OnDisablePrivacyScrimChanged(bool value)
    {
        DebugOptions.DisablePrivacyScrim = value;
    }

    [RelayCommand]
    public static void DeleteAccessToken()
    {
        if (DebugOptions.Enabled)
            TokenHolder.DeleteAccessToken();
    }

    [RelayCommand]
    public static void DeleteRefreshToken()
    {
        if (DebugOptions.Enabled)
            TokenHolder.DeleteRefreshToken();
    }

    [RelayCommand]
    public static async Task ClearRealmData()
    {
        await DebugOptions.ClearRealmData();
    }

    [RelayCommand]
    public static async Task ClearSafetyAssessmentDraft()
    {
        await DebugOptions.ClearSafetyAssessmentDraftsRealm();
    }

    [RelayCommand]
    public static async Task ClearAttachmentDraft()
    {
        await DebugOptions.ClearAttachmentDraftsRealm();
    }

    [RelayCommand]
    public static async Task Logout()
    {
        if (DebugOptions.Enabled)
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
        DebugOptions.OpenAppDataDirectory();
    }

    [RelayCommand]
    public static void OpenCacheDirectory()
    {
        DebugOptions.OpenCacheDirectory();
    }

    [RelayCommand]
    public static void ClearSecureStorage()
    {
        DebugOptions.ClearSecureStorage();
    }

    [RelayCommand]
    public async Task LoadMockPersonVisits()
    {
        await DebugOptions.LoadPersonVisitsMockData(MockPersonVisitsParentId);
    }

    [RelayCommand]
    public static async Task SetWarning()
    {
        await DebugOptions.SetThreshold(VisitDaysThreshold.Warning);
    }

    [RelayCommand]
    public static async Task SetDanger()
    {
        await DebugOptions.SetThreshold(VisitDaysThreshold.Danger);
    }

    [RelayCommand]
    public static async Task SetCritical()
    {
        await DebugOptions.SetThreshold(VisitDaysThreshold.Critical);
    }

    [RelayCommand]
    public static Task ClearOfficeNames()
    {
        return DebugOptions.ClearOfficeNames();
    }

    [RelayCommand]
    public static Task RemoveOneOffice()
    {
        return DebugOptions.RemoveOneOffice();
    }

    [RelayCommand]
    public static async Task RunRecordCleanup()
    {
        await DebugOptions.RunRecordCleanupService();
    }

    [RelayCommand]
    public static void RunAutoCaseloadRefreshService()
    {
        _ = DebugOptions.RunAutoCaseloadRefreshService();
    }

    [RelayCommand]
    public static void ResetAutoCaseloadRefresh()
    {
        DebugOptions.ResetAutoCaseloadRefresh();
    }
}
