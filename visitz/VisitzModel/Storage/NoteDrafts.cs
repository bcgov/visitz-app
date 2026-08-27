using Realms;
using Realms.Schema;
using VisitzModel.Models.Notes;

namespace VisitzModel.Storage;

public class NoteDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "noteDraftRealm.realm";
    public static readonly ulong CurrentVersion = Version3_0_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[] { typeof(NoteDraft) };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateNoteDrafts(migration, oldSchemaVersion);
    }

    static void MigrateNoteDrafts(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < Version2_3_3)
            MigrateTo2_3_3(migration);
        if (oldSchemaVersion < Version3_0_0)
            MigrateTo3_0_0(migration);
    }

    static void MigrateTo2_3_3(Migration migration)
    {
        const string NoteDraftName = "NoteDraft";
        const string DraftCreatedName = "DraftCreated";
        const string LastModifiedName = "LastUpdated";

        var oldItems = migration.OldRealm.DynamicApi.All(NoteDraftName);
        var newItems = migration.NewRealm.DynamicApi.All(NoteDraftName);

        for (int i = 0; i < newItems.Count(); i++)
        {
            var oldDraft = oldItems.ElementAt(i);
            var newDraft = newItems.ElementAt(i);

            newDraft.DynamicApi.Set(DraftCreatedName, DateTimeOffset.MinValue);
            newDraft.DynamicApi.Set(LastModifiedName, DateTimeOffset.MinValue);

            M2_3_3_HandlePrimaryKeyRename(oldDraft, newDraft);
        }
    }

    static void M2_3_3_HandlePrimaryKeyRename(IRealmObject old, IRealmObject @new)
    {
        const string CaseIncidentAndCreatedDateIDName = "CaseIncidentAndCreatedDateID";
        const string ParentEntityIdName = "ParentEntityId";

        var oldId = old.DynamicApi.Get<string>(CaseIncidentAndCreatedDateIDName);
        @new.DynamicApi.Set(ParentEntityIdName, oldId);
    }

    static void MigrateTo3_0_0(Migration migration)
    {
        MapAll<NoteDraft>(
            "NoteDraft",
            migration,
            (n, o) =>
            {
                n.ParentEntityId = o.DynamicApi.Get<string>("ParentEntityId") ?? string.Empty;
                n.RelatedEntityTypeInt = o.DynamicApi.Get<int>("RelatedEntityTypeInt");
                n.RelatedEntitySubtypeInt = o.DynamicApi.Get<int>("RelatedEntitySubtypeInt");
                n.Draft = o.DynamicApi.Get<string>("Draft") ?? string.Empty;
                n.DraftCreated = o.DynamicApi.Get<DateTimeOffset>("DraftCreated");
                n.LastUpdated = o.DynamicApi.Get<DateTimeOffset>("LastUpdated");
                n.DraftLocation = o.DynamicApi.Get<string>("DraftLocation") ?? string.Empty;
            }
        );
    }
}
