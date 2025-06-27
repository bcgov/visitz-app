using Realms;

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
    }

    private static void MigratePersonVisits(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_7_1)
            PersonVisitMigrations.Migrate_2_7_1(migration);
    }
}
