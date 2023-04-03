using hestia.ViewModels;
using hestia.Routers;

namespace hestia.Views;

public partial class OpenIdAuthenticationPage : BasePage
{
    private OpenIdAuthenticationViewModel viewModel;
    private OpenIdAuthenticationRouter router;

    public OpenIdAuthenticationPage(OpenIdAuthenticationViewModel viewModel, OpenIdAuthenticationRouter router)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.router = router;
    }

    protected override async void OnLoad()
    {
        base.OnLoad();
        OpenIdAuthenticationViewModel.Result result = await viewModel.Authenticate();
        router.routeUsing(result);
    }
}
