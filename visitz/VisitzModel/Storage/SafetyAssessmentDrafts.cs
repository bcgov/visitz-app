using Realms;
using Realms.Schema;
using VisitzModel.Models.SafetyAssess;

namespace VisitzModel.Storage;

public class SafetyAssessmentDrafts(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "safetyAssessmentRealmPath.realm";
    public static readonly ulong CurrentVersion = Version2_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
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
        /* Nothing... yet. */
    }
}
