using Realms;
using System.Runtime.CompilerServices;
using VisitzModelTest.Models.Caseload;

namespace VisitzModelTest;

internal class TestingUtilities
{
    public static async Task<Realm> MakeRealm([CallerMemberName] string caller = "")
    {
        return await Realm.GetInstanceAsync(
            new InMemoryConfiguration(nameof(CaseRecordTests) + caller));
    }
}
