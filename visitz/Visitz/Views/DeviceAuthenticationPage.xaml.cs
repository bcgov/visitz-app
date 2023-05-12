using Visitz.ViewModels;
using Visitz.Services;
using Visitz.Routers;
namespace Visitz.Views;

public partial class DeviceAuthenticationPage : VisitzPage
{
    DeviceAuthenticationViewModel viewModel;
    DeviceAuthenticationRouter router;

    public DeviceAuthenticationPage(DeviceAuthenticationViewModel viewModel, DeviceAuthenticationRouter router)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.router = router;
    }

    protected override async void OnLoad()
    {
        base.OnLoad();
        DeviceAuthenticator.Result result = await viewModel.Authenticate();
        router.RouteUsing(result);
    }
}
