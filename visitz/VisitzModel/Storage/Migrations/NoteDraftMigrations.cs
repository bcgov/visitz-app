using Realms;

namespace VisitzModel.Storage.Migrations;

public static class NoteDraftMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateNoteDrafts(migration, oldSchemaVersion);
    }

    static void MigrateNoteDrafts(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_3_3)
            MigrateTo2_3_3(migration);
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
}
