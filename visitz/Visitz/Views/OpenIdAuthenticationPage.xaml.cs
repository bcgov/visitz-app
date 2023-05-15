using Visitz.ViewModels;
using Visitz.Routers;

namespace Visitz.Views;

public partial class OpenIdAuthenticationPage : VisitzPage
{
    private OpenIdAuthenticationViewModel viewModel;
    private OpenIdAuthenticationRouter router;

    public OpenIdAuthenticationPage(OpenIdAuthenticationViewModel viewModel, OpenIdAuthenticationRouter router) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.router = router;
    }

    protected override async void OnLoad()
    {
        OpenIdAuthenticationViewModel.Result result = await viewModel.Authenticate();
        router.routeUsing(result);
    }
}
