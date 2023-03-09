using System;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace hestia.Services
{
    /// <summary>
    /// Authenticate a user via fingerprint, face id or any other biometic / local authentication method.
    /// </summary>
	public class DeviceAuthenticator
	{
        public async Task<Result> Authenticate()
        {
            var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();
            if (isAvailable)
            {
                var request = new AuthenticationRequestConfiguration("Login using biometrics",
                    "Confirm access using biometrics/pattern/passcode")
                {
                    FallbackTitle = "Use PIN/Pattern",
                    AllowAlternativeAuthentication = true,
                };
                var result = await CrossFingerprint.Current.AuthenticateAsync(request);
                if (result.Authenticated)
                {
                    return Result.Successful;
                }
                else
                {
                    return Result.Failure;
                }
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

