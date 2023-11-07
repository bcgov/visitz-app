using Visitz.Resources.Localization;

namespace Visitz.Network;

public static class NetworkHelper
{
    public static bool InternetAvailable => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public static void AssertInternetAvailable(string messageIfUnavailable = null)
    {
        messageIfUnavailable ??= LocalizedStrings.NoInternet;

        MainThread.BeginInvokeOnMainThread(delegate
        {
            // Forcing Internet check on main thread to avoid issue on Windows:
            // https://github.com/dotnet/maui/issues/9972
            if (!InternetAvailable)
                throw new InternetUnavailableException(messageIfUnavailable);
        });
    }
}
