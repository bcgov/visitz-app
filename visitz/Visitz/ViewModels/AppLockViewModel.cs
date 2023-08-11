using Visitz.Authentication;
using Visitz.Pages;
using Visitz.Resources.Localization;

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
            base.PageStarted();

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
                    await Page.DisplayAlert(
                        LocalizedStrings.EnableDeviceSecurity,
                        LocalizedStrings.SecureDeviceAndTryAgain,
                        LocalizedStrings.Ok
                    );
                    break;
                case DeviceAuthenticator.Result.Successful:
                    await VisitzApp.Navigation.PopModalAsync();
                    break;
            }
        }
    }
}

