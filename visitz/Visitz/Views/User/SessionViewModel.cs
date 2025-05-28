using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using DisplayOptions = Visitz.Views.FeaturedBackgroundUnderlay.DisplayOptions;

#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Views.User;

public partial class SessionViewModel(ILogger<SessionViewModel> logger) : VisitzViewModel
{
    [ObservableProperty]
    public string buildNumber;

    [ObservableProperty]
    public string appVersion;

    [ObservableProperty]
    public string backgroundImageUri;

    [ObservableProperty]
    public DisplayOptions bgDisplayOptions = DisplayOptions.Clear;

    [ObservableProperty]
    public bool showLoginLayout;

#if IOS
    private static readonly UIModalPresentationStyle DialogStyle = UIModalPresentationStyle.PageSheet;

    [ObservableProperty]
    public UIModalPresentationStyle presentationStyle;
#else
    [ObservableProperty]
    public object presentationStyle;
#endif

    private OidcSessionInfo SessionInfo;

    private ILogger<SessionViewModel> Logger { get; } = logger;

    public static async Task<string> GetDisplayNamePrompt(OidcSessionInfo info = null)
    {
        info ??= await OidcSessionInfo.GetAsync();
        return info.DisplayName?.Length > 0 ? info.DisplayName : LocalizedStrings.Login;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        BuildNumber = AppInfo.Current.BuildString;
        AppVersion = AppInfo.Current.VersionString;

        SessionInfo = await OidcSessionInfo.GetAsync();
        await ApplyLayout();

        OidcSession.SessionChanged += OidcSession_SessionChanged;

        BackgroundImageUri = BcGovAlbum.GetFeaturedPictureUri();
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

    private void ApplyLoginLayout()
    {
        ShowLoginLayout = true;
        ShowAuthStatusLayout = !ShowLoginLayout;
        IsAuthorized = false;
        IsUnauthorized = false;
        BgDisplayOptions = DisplayOptions.Clear;

#if IOS
        ApplyModalStyles(false);
#endif
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

    [RelayCommand]
    public static void TryLogout()
    {
        _ = PromptAndLogoutAsync();
    }

    private static async Task PromptAndLogoutAsync()
    {
        if (await PromptLogout())
            await OidcSession.LogoutAsync();
    }

    private static async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlert(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel);
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
