using Realms;

namespace Visitz.Storage
{
    public class VisitzRealm
    {
        private static readonly string DbName = "visitz.realm";

        public static async Task<Realm> GetAsync()
        {
            return await Realm.GetInstanceAsync(new RealmConfiguration(DbName)
            {
                EncryptionKey = await VisitzKey.GetKey()
            });
        }
    }
}
