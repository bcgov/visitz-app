using Visitz.Routers;
using Visitz.Services;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the local device authentication(Biometrics, PIN, Pattern etc) goes here.
    /// </summary>
	public class DeviceAuthenticationViewModel : VisitzViewModel
    {
        DeviceAuthenticationRouter router;
        DeviceAuthenticator authenticator;

        public DeviceAuthenticationViewModel(DeviceAuthenticationRouter router, DeviceAuthenticator authenticator)
        {
            this.router = router;
            this.authenticator = authenticator;
        }

        public override async void PageStarted()
        {
            DeviceAuthenticator.Result result = await authenticator.Authenticate();
            router.RouteUsing(result);
        }
    }
}

