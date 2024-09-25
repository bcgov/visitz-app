using Oidc;
using Visitz.Auth;
using Visitz.Resources.Localization;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.User;

public partial class SessionPage : VisitzPage
{
    public static bool IsOpen => Navigator.Navigation.ModalStack
        .Any(page => page.GetType() == typeof(SessionPage));

    public SessionPage(SessionViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	public static async Task TryOpenAsync(Page fromPage = null, bool modal = false, bool animated = true)
	{
        if (IsOpen || await OidcSession.HasRole(VisitzRoles.BasicAccess))
            return;

        await Navigator.GoToPage<SessionPage>(fromPage, modal: modal, animated: animated);
	}

    protected override bool OnBackButtonPressed()
    {
		return AppLockPage.BackButtonEnabled;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var message = new EmailMessage
        {
            To = [(ViewModel as SessionViewModel).MailToUrl],
            Subject = LocalizedStrings.AuthorizationRequest,
        };

        await Email.Default.ComposeAsync(message);
    }
}
