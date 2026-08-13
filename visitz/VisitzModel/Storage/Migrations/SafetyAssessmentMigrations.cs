using Realms;

namespace VisitzModel.Storage.Migrations;

internal static class SafetyAssessmentMigrations
{
    internal static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_3_3)
            Migrate2_3_3(migration);
    }

    static void Migrate2_3_3(Migration migration)
    {
        const string SafetyAssessmentName = "SafetyAssessment";
        const string AssessmentDraftName = "AssessmentDraft";

        const string IncidentNumberName = "IncidentNumber";
        const string DraftCreatedName = "DraftCreated";

        const string LastCreatedName = "LastUpdated";

        static void Create(Migration migration, IRealmObject oldItem)
        {
            string pk = oldItem.DynamicApi.Get<string>(IncidentNumberName);
            var newDraft = migration.NewRealm.DynamicApi.CreateObject(AssessmentDraftName, pk);

            newDraft.DynamicApi.Set(DraftCreatedName, DateTimeOffset.MinValue);
            newDraft.DynamicApi.Set(LastCreatedName, DateTimeOffset.MinValue);
        }


        var oldItems = migration.OldRealm.DynamicApi.All(SafetyAssessmentName);

        for (int i = 0; i < oldItems.Count(); i++)
            Create(migration, oldItems.ElementAt(i));
    }
}
