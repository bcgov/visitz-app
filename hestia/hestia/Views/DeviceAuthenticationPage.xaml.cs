using hestia.ViewModels;
using hestia.Services;
using hestia.Routers;
namespace hestia.Views;

public partial class DeviceAuthenticationPage : BasePage
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
