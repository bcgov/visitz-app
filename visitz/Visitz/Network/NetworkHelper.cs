using Visitz.Resources.Localization;

namespace Visitz.Network;

public static class NetworkHelper
{
    public static bool InternetAvailable => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public static void AssertInternetAvailable(string messageIfUnavailable = null)
    {
        messageIfUnavailable ??= LocalizedStrings.NoInternet;

        if (!InternetAvailable)
            throw new InternetUnavailableException(messageIfUnavailable);
    }
}
