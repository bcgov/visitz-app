using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oidc;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Services;
using Visitz.Settings;
using Visitz.Storage;


#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.ViewModels;

public partial class SessionViewModel : VisitzViewModel
{
    private static readonly double ShowcaseOpacity = 0.8d;
    private static readonly double ReadableOpacity = 0.4d;

    [ObservableProperty]
    public string buildNumber;

    [ObservableProperty]
    public string appVersion;

    [ObservableProperty]
    public string backgroundImageUri;

    [ObservableProperty]
    public double bgOpacity = ShowcaseOpacity;

    private OidcSessionInfo SessionInfo;

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

        OidcSession.SessionChanged += VisitzSession_SessionChanged;

        BackgroundImageUri = await BcGovAlbum.GetFeaturedPictureUri();
    }

    public override void Destroy()
    {
        OidcSession.SessionChanged -= VisitzSession_SessionChanged;

        base.Destroy();
    }

    private async void VisitzSession_SessionChanged(object sender, EventArgs e)
    {
        SessionInfo = sender as OidcSessionInfo;
        await ApplyLayout();
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
        BgOpacity = ShowcaseOpacity;

        ApplyModalStyles(false);
    }

    [RelayCommand]
    public async void LoginAsync()
    {
        try
        {
            await OidcSession.LoginAsync(messageIfUnavailable: LocalizedStrings.NoInternet);

            if (SessionInfo.HasBasicAccessRole())
            {
                await Navigator.Navigation.PopModalAsync();
                WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage());
            }
        }
        catch
        {
            // TODO: log or show error
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

    private void ApplyAuthStatusLayout()
    {
        DisplayName = SessionInfo.GivenName;
        IsAuthorized = SessionInfo.HasBasicAccessRole();
        IsUnauthorized = !IsAuthorized;
        MailToUrl = new AppSettings().ContactInfo.MailToAuthorize;

        if (IsUnauthorized)
        {
            AuthStatus = LocalizedStrings.LoginSuccessButUnauth;
            AuthIcon = MaterialIcons.Shield_lock;
            AuthColor = VisitzColors.BC_Semantic_Error;
            BgOpacity = ReadableOpacity;
        }
        else
        {
            AuthStatus = LocalizedStrings.YouAreAuthorized;
            AuthIcon = MaterialIcons.Verified_user;
            AuthColor = VisitzColors.BC_Semantic_Success;
            BgOpacity = ShowcaseOpacity;
        }

        ShowLoginLayout = false;
        ShowAuthStatusLayout = !ShowLoginLayout;

        ApplyModalStyles(true);
    }

    [RelayCommand]
    public async void LogoutAsync()
    {
        await DoLogoutAsync();
    }

    [RelayCommand]
    public async void TryLogoutAsync()
    {
        if (await PromptLogout())
            await DoLogoutAsync();
    }

    private async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
    }

    private async Task DoLogoutAsync()
    {
        bool reopen = ShouldReopen();

        await OidcSession.LogoutAsync();
        await (await VisitzRealms.GetIcmDataAsync()).ClearAllData();

        if (reopen)
        {
            await Navigator.Navigation.PopModalAsync();
            await Navigator.GoToPage<SessionPage>(modal: true);
        }
    }

    [RelayCommand]
    private async void RequestAccessAsync()
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
    private async void ClosePage()
    {
        await Navigator.Navigation.PopModalAsync();
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

    private bool ShouldReopen()
    {
#if IOS
        return PresentationStyle == DialogStyle;
#else
        return false;
#endif
    }
}