using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Settings;
using DisplayOptions = Visitz.Views.FeaturedBackgroundUnderlay.DisplayOptions;

#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Views.User;

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

#if IOS
        ApplyModalStyles(true);
#endif
    }

#if IOS
    private void ApplyModalStyles(bool sessionExists)
    {
        PresentationStyle = sessionExists && SessionInfo.HasBasicAccessRole()
            ? DialogStyle
            : UIModalPresentationStyle.FullScreen;
    }
#endif

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
        _ = DoOpenCollectionNotice();
    }

    static async Task DoOpenCollectionNotice()
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
