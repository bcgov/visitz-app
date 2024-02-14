using Realms;
using Realms.Schema;
using VisitzModel.Models;

namespace VisitzModel.Storage;

public class NoteDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "noteDraftRealm.realm";
    public static readonly ulong CurrentVersion = Version2_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(NoteDraft), };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        /* Nothing... yet. */
    }
}
