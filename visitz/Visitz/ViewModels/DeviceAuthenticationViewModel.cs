using Visitz.Resources.Localization;
using Visitz.Routers;
using Visitz.Services;
using Visitz.Views;

namespace Visitz.ViewModels
{
	public class DeviceAuthenticationViewModel : VisitzViewModel
    {
        DeviceAuthenticator authenticator;

        public DeviceAuthenticationPage Page => (DeviceAuthenticationPage)VisitzPage;

        public DeviceAuthenticationViewModel(DeviceAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        public override async void PageStarted()
        {
            await PromptAuthentication();
        }

        private async Task PromptAuthentication()
        {
            DeviceAuthenticator.Result result = await authenticator.Authenticate();
            await RouteUsing(result);
        }

        public async Task RouteUsing(DeviceAuthenticator.Result result)
        {
            switch (result)
            {
                case DeviceAuthenticator.Result.NotConfigured:
                    await Page.DisplayAlert(
                        LocalizedStrings.UnprotectedDevice,
                        LocalizedStrings.SecureDeviceAndTryAgain,
                        LocalizedStrings.Ok
                    );
                    break;
                case DeviceAuthenticator.Result.Successful:
                    await Shell.Current.Navigation.PopAsync();
                    await NavigateTo(typeof(OpenIdAuthenticationPage));
                    break;
                case DeviceAuthenticator.Result.Failure:
                    // TODO: using dynamic visibility, show message in Page prompting for auth, show button to open auth prompt
                    break;
            }
        }
    }
}

