using Realms;
using System.Runtime.CompilerServices;

namespace VisitzModelTest;

internal class TestingUtilities
{
    public static async Task<Realm> MakeRealm<T>([CallerMemberName] string caller = "")
    {
        return await Realm.GetInstanceAsync(
            new InMemoryConfiguration(nameof(T) + caller));
    }
}
