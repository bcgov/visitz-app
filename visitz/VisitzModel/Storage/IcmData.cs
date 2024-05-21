using Realms;
using Realms.Schema;
using VisitzModel.Models;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public class IcmData(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "icmDataCopies.realm";
    public static readonly ulong CurrentVersion = Version2_3_3;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(CaseloadItem),
            typeof(FamilyMember),
            typeof(NoteItem),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        IcmDataMigrations.MigrateRealm(migration, oldSchemaVersion);
    }
}
