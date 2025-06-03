using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;
using DisplayOptions = Visitz.Views.FeaturedBackgroundUnderlay.DisplayOptions;

namespace Visitz.Views.User;

public partial class SessionViewModel(ILogger<SessionViewModel> logger) :
    VisitzViewModel,
    IRecipient<ServiceStateMessage>,
    IRecipient<AppLockMessage>
{
    [ObservableProperty]
    public string buildNumber = AppInfo.Current.BuildString;

    [ObservableProperty]
    public string appVersion = AppInfo.Current.VersionString;

    [ObservableProperty]
    public DisplayOptions bgDisplayOptions = DisplayOptions.Clear;

    [ObservableProperty]
    public bool showLoginLayout;

    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public bool showAuthStatusLayout;

    [ObservableProperty]
    public string authStatus;

    [ObservableProperty]
    public bool showAuthStatus;

    [ObservableProperty]
    public bool isAuthorized;

    [ObservableProperty]
    public bool isUnauthorized;

    [ObservableProperty]
    public bool tryingAuthorization;

    [ObservableProperty]
    public bool showButtons;

    [ObservableProperty]
    public bool showUnknown;

    public Action AuthorizationSuccess { get; set; }

    private OidcSessionInfo SessionInfo;

    private ILogger<SessionViewModel> Logger { get; } = logger;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        SessionInfo = await OidcSessionInfo.GetAsync();

        if (await ApplyLayoutByAuthStatus() is (bool, _) sessionStatus)
        {
#if IOS
            // Having issues with lifecycle timings on iOS and this delay solves
            // it wonderfully. Not ideal but it works.
            await Task.Delay(100);
#endif
            if (!AppLockPage.IsOpen && sessionStatus.SessionExists)
                // If AppLockPage is open, it will auto prompt to authenticate.
                // This will cause an error if VisitzApiService needs to prompt
                // user for login, and the user will be stuck at a blank screen
                // in this page.
                DownloadCaseloadAndSubscribe();
        }

        StrongReferenceMessenger.Default.Register<AppLockMessage>(this);
        OidcSession.SessionChanged += OidcSession_SessionChanged;
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            OidcSession.SessionChanged -= OidcSession_SessionChanged;
            StrongReferenceMessenger.Default.UnregisterAll(this);
            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void SetUiOptions(
        bool showLoginLayout = false,
        bool showAuthStatusLayout = false,
        bool showAuthStatus = false,
        bool isAuthorized = false,
        bool isUnauthorized = false,
        bool tryingAuthorization = false,
        bool? showButtons = null,
        bool showUnknown = false)
    {
        ShowLoginLayout = showLoginLayout;
        ShowAuthStatusLayout = showAuthStatusLayout;
        ShowAuthStatus = showAuthStatus;
        IsAuthorized = isAuthorized;
        IsUnauthorized = isUnauthorized;
        TryingAuthorization = tryingAuthorization;
        ShowButtons = showButtons ?? (showLoginLayout ? false : !IsAuthorized || IsUnauthorized);
        ShowUnknown = showUnknown;

        BgDisplayOptions = showLoginLayout
            ? DisplayOptions.Clear
            : DisplayOptions.TextReadable;

        if (tryingAuthorization)
            AuthStatus = LocalizedStrings.CheckingIcmProfile;
        else if (isUnauthorized)
            AuthStatus = LocalizedStrings.LoginSuccessButUnauth;
        else if (!isAuthorized)
            AuthStatus = "MCFD Mobility needs to confirm you have a valid ICM profile.";
        else
            AuthStatus = string.Empty;
    }

    private async Task<bool?> ApplyAuthStatusLayout()
    {
        bool? isAuthorized = await OidcSession.IsAuthorized();

        SetUiOptions(
            showAuthStatusLayout: true,
            showAuthStatus: true,
            isAuthorized: isAuthorized ?? false,
            isUnauthorized: !isAuthorized ?? false,
            showUnknown: isAuthorized == null,
            showButtons: true);

        DisplayName = SessionInfo.GivenName;

        return isAuthorized;
    }

    private async void OidcSession_SessionChanged(object sender, SessionChangedEventArgs e)
    {
        SessionInfo = sender as OidcSessionInfo;
        await ApplyLayoutByAuthStatus();
    }

    private async Task<(bool SessionExists, bool? IsAuthorized)> ApplyLayoutByAuthStatus()
    {
        if (await OidcSession.SessionExistsAsync())
            return (true, await ApplyAuthStatusLayout());
        else
        {
            SetUiOptions(showLoginLayout: true);
            return (false, null);
        }
    }

    [RelayCommand]
    public void Login()
    {
        _ = LoginAsync();
    }

    public async Task LoginAsync()
    {
        try
        {
            var cancelToken = new CancellationTokenSource();
#if WINDOWS
            (Application.Current as VisitzApp).AuthCancelTokenSource = cancelToken;
#endif
            await OidcSession.LoginAsync(messageIfUnavailable: LocalizedStrings.NoInternet, cancelToken.Token);

            await ApplyAuthStatusLayout();

            DownloadCaseloadAndSubscribe();
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
                Logger.LogError(ex, ex.Message);
        }
    }

    [RelayCommand]
    public void TryLogout()
    {
        _ = PromptAndLogoutAsync();
    }

    private async Task PromptAndLogoutAsync()
    {
        if (await PromptLogout())
        {
            SetUiOptions(showLoginLayout: true);
            await OidcSession.LogoutAsync();
        }
    }

    private static async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
    }

    [RelayCommand]
    public void DownloadCaseloadAndSubscribe()
    {
        WeakReferenceMessenger.Default.Register<ServiceStateMessage, string>(this, GetCaseloadService.MakeId());

        var msg = GetAllDataForOfflineService.MakeStartMessage(forceDownload: true);
        WeakReferenceMessenger.Default.Send(msg);

        // extra SetUiOptions call before Receive() so "unauthorized" UI
        // doesn't flash
        SetUiOptions(
            showAuthStatusLayout: true,
            showAuthStatus: true,
            tryingAuthorization: true); 
    }

    public void Receive(ServiceStateMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (message.Status == VisitzService.State.Running)
            {
                SetUiOptions(
                    showAuthStatusLayout: true,
                    showAuthStatus: true,
                    tryingAuthorization: true);
            }
            else
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);

                if (message.Result == VisitzService.Result.Successful)
                    AuthorizationSuccess();
                else
                {
                    SetUiOptions(
                        showAuthStatusLayout: true,
                        showAuthStatus: true,
                        isUnauthorized: true);
                }
            }
        });
    }

    public void Receive(AppLockMessage message)
    {
        if (message.Value == AppLockStatus.Closed)
            DownloadCaseloadAndSubscribe();
    }
}
