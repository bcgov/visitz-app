using Oidc;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.User;

public partial class SessionPage : VisitzPage
{
    public static bool IsOpen
    {
        get
        {
            static bool isSessionPage(Page page) => page.GetType() == typeof(SessionPage);
            return Navigator.Navigation.ModalStack.Any(isSessionPage)
                || Navigator.Navigation.NavigationStack.Any(isSessionPage);
        }
    }

    public SessionPage(SessionViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.AuthorizationSuccess = () => Navigator.Navigation.RemovePage(this);
    }

    public static async Task TryOpenAsync(
        Page fromPage = null,
        bool modal = false,
        bool animated = true)
    {
        if (IsOpen
            || await OidcSession.SessionExistsAsync()
            && (await OidcSession.IsAuthorizedAsync() ?? false))
            return;

        await Navigator.GoToPage<SessionPage>(fromPage, modal: modal, animated: animated);
    }

    protected override bool OnBackButtonPressed()
    {
        return AppLockPage.BackButtonEnabled;
    }
}
