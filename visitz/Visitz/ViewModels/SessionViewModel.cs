using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
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
    [ObservableProperty]
    public string buildNumber;

    [ObservableProperty]
    public string appVersion;

    [ObservableProperty]
    public string backgroundImageUri;

    private VisitzSessionInfo SessionInfo;

    public static async Task<string> GetDisplayNamePrompt(VisitzSessionInfo info = null)
    {
        info ??= await VisitzSessionInfo.GetAsync();
        return info.DisplayName?.Length > 0 ? info.DisplayName : LocalizedStrings.Login;
    }

    public override async void PageCreated()
    {
        base.PageCreated();

        BuildNumber = AppInfo.Current.BuildString;
        AppVersion = AppInfo.Current.VersionString;

        SessionInfo = await VisitzSessionInfo.GetAsync();
        await ApplyLayout();

        VisitzSession.SessionChanged += VisitzSession_SessionChanged;

        BackgroundImageUri = await BcGovAlbum.GetFeaturedPictureUri();
    }

    public override void PageDestroyed()
    {
        VisitzSession.SessionChanged -= VisitzSession_SessionChanged;

        base.PageDestroyed();
    }

    private async void VisitzSession_SessionChanged(object sender, EventArgs e)
    {
        SessionInfo = sender as VisitzSessionInfo;
        await ApplyLayout();
    }

    private async Task ApplyLayout()
    {
        if (await VisitzSession.SessionExistsAsync())
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

        ApplyModalStyles(false);
    }

    [RelayCommand]
    public async void LoginAsync()
    {
        try
        {
            await VisitzSession.LoginAsync();

            if (SessionInfo.HasBasicAccessRole)
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
        IsAuthorized = SessionInfo.HasBasicAccessRole;
        IsUnauthorized = !IsAuthorized;
        MailToUrl = new AppSettings().ContactInfo.MailToAuthorize;

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
        return await VisitzPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
    }

    private async Task DoLogoutAsync()
    {
        bool reopen = ShouldReopen();

        await VisitzSession.LogoutAsync();
        await VisitzRealm.ClearIcmDataRealm();

        if (reopen)
        {
            await Navigator.Navigation.PopModalAsync();
            await SessionPage.OpenAsync(modal: true);
        }
    }

    [RelayCommand]
    private async void RequestAccessAsync()
    {
        await VisitzPage.DisplayAlert(
            "Request access", 
            "Feature not implemented yet.", 
            LocalizedStrings.Ok);
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
        PresentationStyle = sessionExists && SessionInfo.HasBasicAccessRole
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