using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Views;

namespace Visitz.ViewModels
{
	public class AppLockViewModel : VisitzViewModel
    {
        private DeviceAuthenticator Authenticator { get; }

        public AppLockPage Page => (AppLockPage)VisitzPage;

        public AppLockViewModel(DeviceAuthenticator authenticator)
        {
            Authenticator = authenticator;
        }

        public override async void PageStarted()
        {
            await PromptAuthentication();
        }

        private async Task PromptAuthentication()
        {
            DeviceAuthenticator.Result result = await Authenticator.Authenticate();
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
                    await NavigateTo(typeof(LoginPage));
                    break;
                case DeviceAuthenticator.Result.Failure:
                    // TODO: using dynamic visibility, show message in Page prompting for auth, show button to open auth prompt
                    break;
            }
        }
    }
}

