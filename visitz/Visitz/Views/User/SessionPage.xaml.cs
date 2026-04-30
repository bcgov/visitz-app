using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;

namespace Visitz.Views.User;

public partial class SessionPage : VisitzPage<SessionPage, SessionViewModel>
{
    static readonly SemaphoreSlim _semaphore = new(1);

    public static bool IsOpen
    {
        get
        {
            static bool isSessionPage(Page page) => page.GetType() == typeof(SessionPage);
            return Navigator.Navigation.ModalStack.Any(isSessionPage)
                || Navigator.Navigation.NavigationStack.Any(isSessionPage);
        }
    }

    public SessionPage(SessionViewModel viewModel, ILogger<SessionPage> logger)
        : base(viewModel, logger)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.AuthorizationSuccess = () => Navigator.Navigation.RemovePage(this);
    }

    public static async Task TryOpenAsync(Page fromPage = null, bool modal = false, bool animated = true)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (
                IsOpen
                || await OidcSession.SessionExistsAsync()
                    && (await OidcSession.IsAuthorizedAsync() ?? false)
                    && (!await OidcSession.IsSessionStale(DebugOptions.StaleThresholdMinutes) ?? false)
            )
                return;

            await Navigator.GoToPage<SessionPage>(fromPage, modal: modal, animated: animated);
        }
        finally
        {
            try
            {
                _semaphore.Release();
            }
            catch { }
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return AppLockPage.BackButtonEnabled;
    }
}
