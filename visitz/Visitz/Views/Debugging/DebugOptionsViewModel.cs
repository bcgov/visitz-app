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
    public partial DebugOptions Options { get; set; } = DebugOptions.Default;

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

    readonly LastUpdatedPrefs lastUpdatedPrefs = ServiceProvider.GetService<LastUpdatedPrefs>();

    [ObservableProperty]
    public partial DateTime CaseloadLastUpdated { get; set; }

    [ObservableProperty]
    public partial DateTime MaxDate { get; set; } = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public partial string MockPersonVisitsParentId { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        AppId = AppInfo.Current.PackageName;
        DotnetVersion = Environment.Version.ToString();

#if DEBUG
        BuildingInDebug = true;
#else
        BuildingInDebug = false;
#endif
        var settings = new AppSettings();

        ApiDomain = settings.Api.ApiDomain;
        AuthenticationDomain = settings.Oidc.AuthenticationDomain;

        CaseloadLastUpdated = lastUpdatedPrefs.Get(GetCaseloadService.MakeId(), DateTimeExtensions.LocalNow);

        if (Application.Current.Windows[0] is Window window)
        {
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
            Options.WindowHeight = window.Height;
            Options.WindowWidth = window.Width;
        }
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
