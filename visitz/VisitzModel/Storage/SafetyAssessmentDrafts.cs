using Realms;
using Realms.Schema;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public class SafetyAssessmentDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "safetyAssessmentRealmPath.realm";
    public static readonly ulong CurrentVersion = Version3_0_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(AssessmentDraft),
            typeof(SafetyAssessment),
            typeof(FactorInfluence),
            typeof(ProtectiveCapacity),
            typeof(SafetyDecisions),
            typeof(SafetyFactors),
            typeof(SafetyInterventions),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        SafetyAssessmentMigrations.MigrateRealm(migration, oldSchemaVersion);

        if (oldSchemaVersion < Version3_0_0)
        {
            MapAll<AssessmentDraft>(
                "AssessmentDraft",
                migration,
                (n, o) =>
                {
                    n.DraftEntityId = o.DynamicApi.Get<string>("DraftEntityId") ?? string.Empty;
                    n.DraftCreated = o.DynamicApi.Get<DateTimeOffset>("DraftCreated");
                    n.LastUpdated = o.DynamicApi.Get<DateTimeOffset>("LastUpdated");
                    n.DraftLocation = o.DynamicApi.Get<string>("DraftLocation") ?? string.Empty;
                    n.RelatedEntityTypeInt = o.DynamicApi.Get<int>("RelatedEntityTypeInt");
                    n.RelatedEntitySubtypeInt = o.DynamicApi.Get<int>("RelatedEntitySubtypeInt");
                }
            );
        }
    }
}
