using System;
using visitz.Resources.Localization;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace visitz.Services
{
    /// <summary>
    /// Authenticate a user via fingerprint, face id or any other biometic / local authentication method.
    /// </summary>
	public class DeviceAuthenticator
    {
        public async Task<Result> Authenticate()
        {
            if (await CrossFingerprint.Current.IsAvailableAsync())
            {
                var request = new AuthenticationRequestConfiguration(LocalizedStrings.DeviceAuthTitle,
                    LocalizedStrings.DeviceAuthReason)
                {
                    AllowAlternativeAuthentication = true,
                };

                var result = await CrossFingerprint.Current.AuthenticateAsync(request);

                return result.Authenticated ? Result.Successful : Result.Failure;
            }
            else
            {
                return Result.NotConfigured;
            }
        }

        public enum Result
        {
            NotConfigured,
            Successful,
            Failure
        }
    }
}

