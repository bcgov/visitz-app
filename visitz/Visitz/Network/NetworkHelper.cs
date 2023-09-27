using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitz.Network;

public static class NetworkHelper
{
    public static bool InternetAvailable => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
}
