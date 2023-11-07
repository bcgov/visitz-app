using Visitz.Authentication.Keycloak;
using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class SessionPage : VisitzPage
{
    public static bool IsOpen => Navigator.Navigation.ModalStack
        .Any(page => page.GetType() == typeof(SessionPage));

    public SessionPage(SessionViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	public static async Task OpenAsync(Page fromPage = null, bool modal = false, bool animated = true)
	{
		await NavigateTo<SessionPage>(fromPage, modal: modal, animated: animated);
	}

	public static async Task TryOpenAsync(Page fromPage = null, bool modal = false, bool animated = true)
	{
        if (IsOpen || await VisitzSession.HasBasicAccess())
            return;

        await OpenAsync(fromPage, modal: modal, animated: animated);
	}

    protected override bool OnBackButtonPressed()
    {
		return AppLockPage.BackButtonEnabled;
    }
}