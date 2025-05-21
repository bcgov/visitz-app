using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Services.Caseload;
using Visitz.Settings;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using DisplayOptions = Visitz.Views.FeaturedBackgroundUnderlay.DisplayOptions;

#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Views.User;

public partial class SessionViewModel : VisitzViewModel
{
    [ObservableProperty]
    public string buildNumber;

    [ObservableProperty]
    public string appVersion;

    [ObservableProperty]
    public string backgroundImageUri;

    [ObservableProperty]
    public DisplayOptions bgDisplayOptions = DisplayOptions.Clear;

    private OidcSessionInfo SessionInfo;

    private ILogger<SessionViewModel> Logger { get; }

    public SessionViewModel(ILogger<SessionViewModel> logger)
    {
        Logger = logger;
    }

    public static async Task<string> GetDisplayNamePrompt(OidcSessionInfo info = null)
    {
        info ??= await OidcSessionInfo.GetAsync();
        return info.DisplayName?.Length > 0 ? info.DisplayName : LocalizedStrings.Login;
    }

    public override async void Create()
    {
        base.Create();

        BuildNumber = AppInfo.Current.BuildString;
        AppVersion = AppInfo.Current.VersionString;

        SessionInfo = await OidcSessionInfo.GetAsync();
        await ApplyLayout();

        OidcSession.SessionChanged += OidcSession_SessionChanged;

        BackgroundImageUri = BcGovAlbum.GetFeaturedPictureUri();
    }

    public override void Destroy()
    {
        OidcSession.SessionChanged -= OidcSession_SessionChanged;

        base.Destroy();
    }

    private async void OidcSession_SessionChanged(object sender, SessionChangedEventArgs e)
    {
        SessionInfo = sender as OidcSessionInfo;
        await ApplyLayout();

        if (e is LogoutChangedEventArgs && ShouldReopen())
            _ = ReopenSessionPage();
    }

    private async Task ApplyLayout()
    {
        if (await OidcSession.SessionExistsAsync())
            ApplyAuthStatusLayout();
        else
            ApplyLoginLayout();
    }
}

public partial class SessionViewModel
{
    [ObservableProperty]
    public bool showLoginLayout;

    private void ApplyLoginLayout()
    {
        ShowLoginLayout = true;
        ShowAuthStatusLayout = !ShowLoginLayout;
        IsAuthorized = false;
        IsUnauthorized = false;
        BgDisplayOptions = DisplayOptions.Clear;

        ApplyModalStyles(false);
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

            if (SessionInfo.HasBasicAccessRole())
            {
                await Navigator.PopAllModalsAsync(true);
                WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage(forceDownload: true));
            }
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
                Logger.LogError(ex, ex.Message);
        }
    }
}

public partial class SessionViewModel
{
    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public bool showAuthStatusLayout;

    [ObservableProperty]
    public string authStatus;

    [ObservableProperty]
    public string authIcon;

    [ObservableProperty]
    public Color authColor;

    [ObservableProperty]
    public bool isAuthorized;

    [ObservableProperty]
    public bool isUnauthorized;

    [ObservableProperty]
    public string mailToUrl;

    [ObservableProperty]
    public bool showFeedbackUrl;

    [ObservableProperty]
    public string feedbackUrl;

    private void ApplyAuthStatusLayout()
    {
        BgDisplayOptions = DisplayOptions.TextReadable;

        DisplayName = SessionInfo.GivenName;
        IsAuthorized = SessionInfo.HasBasicAccessRole();
        IsUnauthorized = !IsAuthorized;
        ShowFeedbackUrl = IsAuthorized;

        var contactInfo = new AppSettings().ContactInfo;
        MailToUrl = contactInfo.MailToAuthorize;
        FeedbackUrl = contactInfo.FeedbackSurveyUrl;

        if (IsUnauthorized)
        {
            AuthStatus = LocalizedStrings.LoginSuccessButUnauth;
            AuthIcon = MaterialIcons.Shield_lock;
            AuthColor = VisitzColors.BC_Semantic_Error;
        }
        else
        {
            AuthStatus = LocalizedStrings.YouAreAuthorized;
            AuthIcon = MaterialIcons.Verified_user;
            AuthColor = VisitzColors.BC_Semantic_Success;
        }

        ShowLoginLayout = false;
        ShowAuthStatusLayout = !ShowLoginLayout;

        ApplyModalStyles(true);
    }

    [RelayCommand]
    public static void TryLogout()
    {
        _ = PromptAndLogoutAsync();
    }

    private static async Task PromptAndLogoutAsync()
    {
        if (await PromptLogout())
            await DoLogoutAsync();
    }

    private static async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
    }

    private static async Task DoLogoutAsync()
    {
        await OidcSession.LogoutAsync();
    }

    private static async Task ReopenSessionPage(bool modal = true)
    {
        await Navigator.PopAllModalsAsync(true);
        await Navigator.GoToPage<SessionPage>(modal: modal);
    }

    [RelayCommand]
    private static void RequestAccess()
    {
        _ = DoRequestAccessAsync();
    }

    private static async Task DoRequestAccessAsync()
    {
        var formUrl = new AppSettings().ContactInfo.AccessRequestFormUrl;

        await Browser.Default.OpenAsync(formUrl, new BrowserLaunchOptions
        {
            LaunchMode = BrowserLaunchMode.SystemPreferred,
            TitleMode = BrowserTitleMode.Hide,
            Flags = BrowserLaunchFlags.PresentAsFormSheet,
        });
    }

    [RelayCommand]
    static void OpenCollectionNotice()
    {
        _ = DoOpenFeedbackUrl();
    }

    static async Task DoOpenFeedbackUrl()
    {
        await Navigator.Navigation.PopModalAsync(animated: false);

        var noticeView = ServiceProvider.GetService<CollectionNoticeView>();
        await Navigator.Navigation.PushModalAsync(noticeView, ViewModalSize.Fullscreen);
    }

    [RelayCommand]
    static void OpenFeedbackUrl(string feedbackUrl)
    {
        _ = DoOpenFeedbackUrl(feedbackUrl);
    }

    static async Task DoOpenFeedbackUrl(string feedbackUrl)
    {
        await Browser.Default.OpenAsync(feedbackUrl, new BrowserLaunchOptions
        {
            LaunchMode = BrowserLaunchMode.SystemPreferred,
            TitleMode = BrowserTitleMode.Hide,
            Flags = BrowserLaunchFlags.PresentAsPageSheet,
        });
    }
}

public partial class SessionViewModel
{
#if IOS
    private static readonly UIModalPresentationStyle DialogStyle = UIModalPresentationStyle.PageSheet;

    [ObservableProperty]
    public UIModalPresentationStyle presentationStyle;
#else
    [ObservableProperty]
    public object presentationStyle;
#endif

    private void ApplyModalStyles(bool sessionExists)
    {
#if IOS
        PresentationStyle = sessionExists && SessionInfo.HasBasicAccessRole()
            ? DialogStyle
            : UIModalPresentationStyle.FullScreen;
#endif
    }

    private static bool ShouldReopen()
    {
#if IOS
        return true;
#else
        return false;
#endif
    }
}
