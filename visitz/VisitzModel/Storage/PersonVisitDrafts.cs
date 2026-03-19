using Realms;
using Realms.Schema;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public partial class PersonVisitDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "personVisitDraftsRealm.realm";
    public static readonly ulong CurrentVersion = Version2_7_1;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(PersonVisit), typeof(PersonVisitDraft) };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < Version2_7_1)
            PersonVisitMigrations.Migrate_2_7_1(migration);
    }
}
