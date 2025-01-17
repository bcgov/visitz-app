namespace Oidc.Network;

#if WINDOWS
using Oidc.Util;
using Windows.Networking.Connectivity;
#endif

public static class NetworkHelper
{
    public static bool InternetAvailable
#if WINDOWS
    // WORKAROUND https://github.com/dotnet/maui/issues/22228#issuecomment-2118235512
    // NetworkAccess reported incorrectly on Windows on VPN
    {
        get
        {
            bool available = false;

            try
            {
                available = GetConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
            catch (Exception ex)
            {
                ConsoleTrace.TraceMethod(typeof(NetworkHelper), ex.ToString());
            }

            return available;
        }
    }

    static NetworkConnectivityLevel GetConnectivityLevel()
    {
        TaskCompletionSource src = new();
        NetworkConnectivityLevel level = NetworkConnectivityLevel.None;

        MainThread.BeginInvokeOnMainThread(delegate
        {
            try
            {
                var connectionProfile = NetworkInformation.GetInternetConnectionProfile()
                    ?? throw new InvalidOperationException("Network connection profile unavailable");

                level = connectionProfile.GetNetworkConnectivityLevel();

                src.SetResult();
            }
            catch (Exception ex)
            {
                src.TrySetException(ex);
            }
        });

        src.Task.Wait();
        return level;
    }
#else
        => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
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
