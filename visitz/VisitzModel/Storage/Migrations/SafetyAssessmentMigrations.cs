using Realms;
using VisitzModel.Storage.Migrations.SafetyAssessments;

namespace VisitzModel.Storage.Migrations;

internal static class SafetyAssessmentMigrations
{
    public static void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion < VisitzRealmBase.Version2_3_3)
            SafetyAssessment2_3_3.Migrate(migration);
    }
}
