using Visitz.Device;
using Visitz.Resources.Localization;

namespace Visitz.ViewModels
{
    public partial class AppLockViewModel(DeviceAuthenticator authenticator) : VisitzViewModel
    {
        private DeviceAuthenticator Authenticator { get; } = authenticator;

        public override async void Start()
        {
            base.Start();

            await PromptAuthentication();
        }

        public async Task PromptAuthentication()
        {
            DeviceAuthenticator.Result result = await Authenticator.Authenticate();
            await RouteUsing(result);
        }

        public async Task RouteUsing(DeviceAuthenticator.Result result)
        {
            switch (result)
            {
                case DeviceAuthenticator.Result.NotConfigured:
                    await Navigator.CurrentOpenPage.DisplayAlert(
                        LocalizedStrings.EnableDeviceSecurity,
                        LocalizedStrings.SecureDeviceAndTryAgain,
                        LocalizedStrings.Ok
                    );
                    break;
                case DeviceAuthenticator.Result.Successful:
                    await Navigator.Navigation.PopModalAsync();
                    break;
            }
        }
    }
}

