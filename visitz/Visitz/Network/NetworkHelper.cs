namespace Visitz.Network;

public static class NetworkHelper
{
    public static bool InternetAvailable => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
}
