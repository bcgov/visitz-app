using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Authentication.Keycloak;
using Visitz.Resources.Localization;
using Visitz.Storage;

namespace Visitz.ViewModels;

public partial class UserSessionViewModel : VisitzViewModel
{
    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public bool showGreeting;

    [ObservableProperty]
    public string authStatus;

    [ObservableProperty]
    public string sessionActionText;

    [ObservableProperty]
    public string buildNumber;

    [ObservableProperty]
    public string appVersion;

    [ObservableProperty]
    public string backgroundImageUri;

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

        await ApplyUserSessionInfo();
        BackgroundImageUri = (await BcGovAlbum.GetPictureUris())[17];

        VisitzSession.SessionChanged += VisitzSession_SessionChanged;
    }

    public override void PageDestroyed()
    {
        VisitzSession.SessionChanged -= VisitzSession_SessionChanged;

        base.PageDestroyed();
    }

    private async void VisitzSession_SessionChanged(object sender, EventArgs e)
    {
        await ApplyUserSessionInfo();
    }

    private async Task ApplyUserSessionInfo()
    {
        if (await VisitzSession.SessionExistsAsync())
        {
            var info = await VisitzSessionInfo.GetAsync();

            DisplayName = info.GivenName;
            AuthStatus = GetAuthStatus(info);
            SessionActionText = LocalizedStrings.Logout;
        }
        else
        {
            DisplayName = "";
            AuthStatus = "";
            SessionActionText = LocalizedStrings.Login;
        }
    }

    partial void OnDisplayNameChanged(string value)
    {
        ShowGreeting = value?.Length > 0;
    }

    private string GetAuthStatus(VisitzSessionInfo info)
    {
    }

    [RelayCommand]
    public async void PerformSessionAction()
    {
        if (await VisitzSession.SessionExistsAsync())
            await TryLogout();
        else
            await VisitzSession.LoginAsync();
    }

    private async Task TryLogout()
    {
        if (!await PromptLogout())
            return;

        await VisitzSession.LogoutAsync();
        await VisitzRealm.ClearIcmDataRealm();
    }

    private async Task<bool> PromptLogout()
    {
        return await VisitzPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc, 
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
    }
}
