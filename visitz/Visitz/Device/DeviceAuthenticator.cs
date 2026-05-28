using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace Visitz.Device;

/// <summary>
/// Authenticate a user via fingerprint, face id or any other biometric / local authentication method.
/// </summary>
public class DeviceAuthenticator
{
    public static async Task<FingerprintAuthenticationResult> Authenticate(string title, string reason)
    {
        var request = new AuthenticationRequestConfiguration(title, reason)
        {
            AllowAlternativeAuthentication = true,
        };

        return await CrossFingerprint.Current.AuthenticateAsync(request);
    }

    public static async Task<(bool Available, FingerprintAvailability)> GetAvailabilityAsync()
    {
        var result = await CrossFingerprint.Current.GetAvailabilityAsync(true);

        return (result == FingerprintAvailability.Available, result);
    }
}
