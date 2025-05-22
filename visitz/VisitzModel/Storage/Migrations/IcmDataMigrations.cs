using Realms;

namespace VisitzModel.Storage.Migrations;

public static class IcmDataMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        MigrateCaseloadItems(migration, oldSchemaVersion);
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
}
