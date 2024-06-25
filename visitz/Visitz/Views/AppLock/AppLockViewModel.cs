using Visitz.Device;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Surveys;
using VisitzModel.Storage;

namespace Visitz.Views.AppLock
{
	public partial class AppLockViewModel(DeviceAuthenticator authenticator) : VisitzViewModel
    {
        private DeviceAuthenticator Authenticator { get; } = authenticator;

#if WINDOWS
        public override async void Create()
        {
            base.Create();

			await PromptAuthentication();
		}
#endif

#if !WINDOWS
        public override async void Start()
        {
            base.Start();

			await PromptAuthentication();
        }
#endif

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

					new SurveyFeedbackTracker(Preferences.Default).IncrementTimesAppUnlocked();
					await FeedbackSurveyPage.TryOpen();
                    break;
            }
        }
    }
}

