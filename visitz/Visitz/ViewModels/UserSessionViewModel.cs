using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Authentication.Keycloak;
using Visitz.Resources.Localization;

namespace Visitz.ViewModels;

public partial class UserSessionViewModel : VisitzViewModel
{
    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public bool showGreeting;

    [ObservableProperty]
    public string sessionActionText;

    [ObservableProperty]
    public string buildNumber;

    [ObservableProperty]
    public string appVersion;

    public static async Task<string> GetDisplayNamePrompt(VisitzSessionInfo info = null)
    {
        info ??= await VisitzSessionInfo.GetAsync();
        return info.DisplayName?.Length > 0 ? info.DisplayName : LocalizedStrings.Login;
    }

    public override async void PageCreated()
    {
        BuildNumber = AppInfo.Current.BuildString;
        AppVersion = AppInfo.Current.VersionString;

        await ApplyUserSessionInfo();

        VisitzSession.SessionChanged += VisitzSession_SessionChanged;
    }

    public override void PageDestroyed()
    {
        VisitzSession.SessionChanged -= VisitzSession_SessionChanged;
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
            SessionActionText = LocalizedStrings.TerminateSession;
        }
        else
        {
            DisplayName = "";
            SessionActionText = LocalizedStrings.Login;
        }
    }

    partial void OnDisplayNameChanged(string value)
    {
        ShowGreeting = value?.Length > 0;
    }

    [RelayCommand]
    public static async void PerformSessionAction()
    {
        if (await VisitzSession.SessionExistsAsync())
            await VisitzSession.LogoutAsync();
        else
            await VisitzSession.LoginAsync();
    }
}
