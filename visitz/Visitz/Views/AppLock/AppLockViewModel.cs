using Plugin.Fingerprint.Abstractions;
using Visitz.Device;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Surveys;
using VisitzModel.Storage;

namespace Visitz.Views.AppLock
{
	public partial class AppLockViewModel() : VisitzViewModel
    {
        public override async void Start()
        {
            base.Start();

			await TryPromptAuthentication();
        }

        public async Task TryPromptAuthentication()
        {
            (bool available, _) = await DeviceAuthenticator.GetAvailabilityAsync();

            if (available)
                await PromptBiometricAuth();
        }

        static async Task PromptBiometricAuth()
        {
            var result = await DeviceAuthenticator.Authenticate(
                LocalizedStrings.DeviceAuthTitle,
                LocalizedStrings.DeviceAuthReason);

            await HandleAuthResult(result);
        }

        public static async Task HandleAuthResult(FingerprintAuthenticationResult result)
        {
            switch (result.Status)
            {
                case FingerprintAuthenticationResultStatus.Succeeded:
                    await HandleSuccessfulAuth();
                    break;
                case FingerprintAuthenticationResultStatus.NotAvailable:
                    await Navigator.CurrentOpenPage.DisplayAlert(
                        LocalizedStrings.EnableDeviceSecurity,
                        LocalizedStrings.SecureDeviceAndTryAgain,
                        LocalizedStrings.Ok
                    );
                    break;
                case FingerprintAuthenticationResultStatus.Canceled:
                    /* No-op */
                    break;
                case FingerprintAuthenticationResultStatus.FallbackRequested:
                case FingerprintAuthenticationResultStatus.TooManyAttempts:
                case FingerprintAuthenticationResultStatus.Failed:
                case FingerprintAuthenticationResultStatus.Denied:
                case FingerprintAuthenticationResultStatus.Unknown:
                case FingerprintAuthenticationResultStatus.UnknownError:
                    await Navigator.CurrentOpenPage.DisplayErrorAlert(result.ErrorMessage);
                    break;
                default:
                    throw new ArgumentException($"Unsupported {nameof(FingerprintAuthenticationResult)}");
            }
        }

        static async Task HandleSuccessfulAuth()
        {
            await Navigator.Navigation.PopModalAsync();
            new SurveyFeedbackTracker(Preferences.Default).IncrementTimesAppUnlocked();
            await FeedbackSurveyPage.TryOpen();
        }
    }
}
