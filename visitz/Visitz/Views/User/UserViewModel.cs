using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using Oidc;
using Visitz.Settings;
using Visitz.Resources.Localization;

namespace Visitz.Views.User;

internal partial class UserViewModel : VisitzViewModel
{
    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public string buildNumber = AppInfo.Current.BuildString;

    [ObservableProperty]
    public string appVersion = AppInfo.Current.VersionString;

    [ObservableProperty]
    public string feedbackUrl;

    OidcSessionInfo SessionInfo;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        SessionInfo = await OidcSessionInfo.GetAsync();
        DisplayName = SessionInfo.GivenName;

        var contactInfo = new AppSettings().ContactInfo;
        FeedbackUrl = contactInfo.FeedbackSurveyUrl;

        OidcSession.SessionChanged += OidcSession_SessionChanged;
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            OidcSession.SessionChanged -= OidcSession_SessionChanged;
            disposed = true;
        }
        base.Dispose(disposing);
    }

    [RelayCommand]
    static async Task OpenCollectionNotice()
    {
        await Navigator.Navigation.PopModalAsync(animated: false);

        var noticeView = ServiceProvider.GetService<CollectionNoticeView>();
        await Navigator.Navigation.PushModalAsync(noticeView, ViewModalSize.Fullscreen);
    }

    [RelayCommand]
    static async Task OpenFeedbackUrl(string feedbackUrl)
    {
        await Browser.Default.OpenAsync(feedbackUrl, new BrowserLaunchOptions
        {
            LaunchMode = BrowserLaunchMode.SystemPreferred,
            TitleMode = BrowserTitleMode.Hide,
            Flags = BrowserLaunchFlags.PresentAsPageSheet,
        });
    }

    [RelayCommand]
    public static void TryLogout()
    {
        _ = PromptAndLogoutAsync();
    }

    static async Task PromptAndLogoutAsync()
    {
        if (await PromptLogout())
            await OidcSession.LogoutAsync();
    }

    static async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
    }

    async void OidcSession_SessionChanged(object sender, Oidc.Events.SessionChangedEventArgs e)
    {
        if (!await OidcSession.SessionExistsAsync())
        {
            // Delay was the only thing I could do to get this working. App
            // wasn't playing nice on Windows waiting for WebViewPage to close
            // and this Pop call kept silently failing.
            await Task.Delay(100);

            await GoToLoginScreen();
        }
    }

    static async Task GoToLoginScreen()
    {
        await Navigator.Navigation.PopModalAsync(animated: true);
        await SessionPage.TryOpenAsync(modal: true, animated: true);
    }
}
