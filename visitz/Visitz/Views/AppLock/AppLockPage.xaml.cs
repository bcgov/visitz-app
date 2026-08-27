using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.Extensions;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;

namespace Visitz.Views.AppLock;

public partial class AppLockPage : VisitzPage<AppLockPage, AppLockViewModel>
{
    const double MinHeightShowHero = 600;

    bool _disposed;

    /// <summary>
    /// Back button behavior disabled on purpose. We don't want to let users avoid this authentication check.
    /// </summary>
    public static readonly bool BackButtonEnabled = false;

    public static bool IsOpen => Navigator.CurrentOpenModal?.GetType() == typeof(AppLockPage);

    public bool PromptOnAppearing { get; set; }

    static bool ShouldSkipAppLock
    {
        get
        {
            bool debugSkipActive = false;
            bool isWindows = false;
#if DEBUG
            debugSkipActive = DebugOptions.Default.SkipLocalAuth;
#endif
#if WINDOWS
            isWindows = true;
#endif
            return debugSkipActive || isWindows;
        }
    }

    public AppLockPage(AppLockViewModel viewModel, ILogger<AppLockPage> logger)
        : base(viewModel, logger)
    {
        InitializeComponent();
        BindingContext = viewModel;

        SizeChanged += AppLockPage_SizeChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

#if !WINDOWS
        CurrentWindow.Resumed += AppLockPage_WindowResumed;
#endif

        StrongReferenceMessenger.Default.Send(new AppLockMessage(AppLockStatus.Opened));

        if (PromptOnAppearing)
            _ = DelayPromptAuthentication();
    }

    async Task DelayPromptAuthentication()
    {
        try
        {
            // Not a fan of delays for this but I do not have time to properly dig into why the app lock page does not
            // properly obscure the underlying page on first app launch, and this fixes it.
            await Task.Delay(500);
            await AppLockViewModel.PromptAuthentication();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex.Message);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        CurrentWindow.Resumed -= AppLockPage_WindowResumed;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            SizeChanged -= AppLockPage_SizeChanged;
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public static async Task TryPrompt(bool promptOnAppearing)
    {
        bool noSession = !await OidcSession.SessionExistsAsync();

        if (IsOpen || noSession || ShouldSkipAppLock)
            return;

        var lockPage = ServiceProvider.Current.GetRequiredService<AppLockPage>();
        lockPage.PromptOnAppearing = promptOnAppearing;
        await Navigator.Navigation.PushModalAsync(lockPage, true);
    }

    protected override bool OnBackButtonPressed()
    {
        return BackButtonEnabled;
    }

    private async void Unlock_Clicked(object? sender, EventArgs e)
    {
        await AppLockViewModel.PromptAuthentication();
    }

    public async void AppLockPage_WindowResumed(object? sender, EventArgs eventArgs)
    {
        await AppLockViewModel.PromptAuthentication();
    }

    private void AppLockPage_SizeChanged(object? sender, EventArgs e)
    {
        ViewModel.ShowHeroImage = Height >= MinHeightShowHero;
    }
}
