using Microsoft.Extensions.Logging;
using Oidc;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;

namespace Visitz.Views.User;

public partial class SessionPage : VisitzPage<SessionPage, SessionViewModel>
{
    static readonly SemaphoreSlim s_semaphore = new(1);

    bool _disposed;

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

        SizeChanged += SessionPage_SizeChanged;
        ApplyOrientation();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            SizeChanged -= SessionPage_SizeChanged;
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public static async Task TryOpenAsync(Page fromPage = null, bool modal = false, bool animated = true)
    {
        await s_semaphore.WaitAsync();

        try
        {
            if (
                IsOpen
                || await OidcSession.SessionExistsAsync()
                    && (await OidcSession.IsAuthorizedAsync() ?? false)
                    && (!await OidcSession.IsSessionStale(DebugOptions.Default.StaleThresholdMinutes) ?? false)
            )
                return;

            await Navigator.GoToPage<SessionPage>(fromPage, modal: modal, animated: animated);
        }
        finally
        {
            try
            {
                s_semaphore.Release();
            }
            catch { }
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return AppLockPage.BackButtonEnabled;
    }

    private void SessionPage_SizeChanged(object? sender, EventArgs e)
    {
        ApplyOrientation();
    }

    void ApplyOrientation()
    {
        RotateBehavior.Orientation = Width >= 500 ? ItemsLayoutOrientation.Horizontal : ItemsLayoutOrientation.Vertical;
    }
}
