namespace Oidc.Network;

#if WINDOWS
using Windows.Networking.Connectivity;
#endif

public static class NetworkHelper
{
    public static bool InternetAvailable =>
#if WINDOWS
        // WORKAROUND https://github.com/dotnet/maui/issues/22228#issuecomment-2118235512
        // NetworkAccess reported incorrectly on Windows on VPN
        NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel()
            == NetworkConnectivityLevel.InternetAccess;
#else
        Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
#endif

    public static void AssertInternetAvailable(string messageIfUnavailable)
    {
        MainThread.BeginInvokeOnMainThread(delegate
        {
            // Forcing Internet check on main thread to avoid issue on Windows:
            // https://github.com/dotnet/maui/issues/9972
            if (!InternetAvailable)
                throw new InternetUnavailableException(messageIfUnavailable);
        });
    }
}
