using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Visitz.Resources.Localization;

namespace Visitz.Device
{
    /// <summary>
    /// Authenticate a user via fingerprint, face id or any other biometric / local authentication method.
    /// </summary>
	public class DeviceAuthenticator
    {
        public async Task<Result> Authenticate()
        {
            var request = new AuthenticationRequestConfiguration(LocalizedStrings.DeviceAuthTitle,
                LocalizedStrings.DeviceAuthReason)
            {
                AllowAlternativeAuthentication = true,
            };

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            return result.Authenticated ? Result.Successful : Result.Failure;
        }

        public enum Result
        {
			Unknown = 0,
            NotConfigured = 1,
            Successful = 2,
            Failure = 3,
        }
    }
}

