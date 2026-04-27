using Realms;
using VisitzModel.Models.Caseload;

namespace VisitzModel.Storage.Migrations;

public static class IcmDataMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateCaseloadItems(migration, oldSchemaVersion);
        MigratePersonVisits(migration, oldSchemaVersion);
    }

    private static void MigrateCaseloadItems(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_7_0)
        {
            const string CaseloadItemName = "CaseloadItem";

            var newCaseloadItems = migration.NewRealm.DynamicApi.All(CaseloadItemName);

            migration.NewRealm.RemoveRange(newCaseloadItems);
        }

        if (oldSchemaVersion < VisitzRealmBase.Version2_8_0)
        {
            foreach (var @case in migration.NewRealm.All<CaseRecord>())
                if (@case.Realm != null)
                    @case.UpsertLocalState(@case.Realm, false);

            foreach (var incident in migration.NewRealm.All<IncidentRecord>())
                if (incident.Realm != null)
                    incident.UpsertLocalState(incident.Realm, false);
        }
    }

    private static void MigratePersonVisits(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_7_1)
            PersonVisitMigrations.Migrate_2_7_1(migration);
    }
}
