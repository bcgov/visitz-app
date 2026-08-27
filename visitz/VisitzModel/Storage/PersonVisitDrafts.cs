using Realms;
using Realms.Schema;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public partial class PersonVisitDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "personVisitDraftsRealm.realm";
    public static readonly ulong CurrentVersion = Version3_0_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(PersonVisit), typeof(PersonVisitDraft) };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        PersonVisitMigrations.MigrateRealm(migration, oldSchemaVersion);

        if (oldSchemaVersion < Version3_0_0)
        {
            MapAll<PersonVisitDraft>(
                "PersonVisitDraft",
                migration,
                (n, o) =>
                {
                    n.RelatedEntityId = o.DynamicApi.Get<string>("RelatedEntityId") ?? Guid.NewGuid().ToString();
                    n.RelatedEntityTypeInt = o.DynamicApi.Get<int>("RelatedEntityTypeInt");
                    n.RelatedEntitySubtypeInt = o.DynamicApi.Get<int>("RelatedEntitySubtypeInt");
                    n.DraftLocation = o.DynamicApi.Get<string>("DraftLocation") ?? string.Empty;
                    n.DraftCreated = o.DynamicApi.Get<DateTimeOffset>("DraftCreated");
                    n.LastUpdated = o.DynamicApi.Get<DateTimeOffset>("LastUpdated");
                    // No need to migrate n.Visit
                }
            );
        }
    }
}
