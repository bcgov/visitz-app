using Realms;
using VisitzModel.Extensions;
using VisitzApi.Models.SafetyAssess;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyInterventions : IRealmObject
{
    public bool DirectIntervention { get; set; }
    
    public bool UseOfIndividuals { get; set; }
    
    public bool UseCommAgencies { get; set; }
    
    public bool ProtectVictim { get; set; }
    
    public bool LeaveHome { get; set; }
    
    public bool NonOffendingParent { get; set; }
    
    public bool LegalIntPlanned { get; set; }
    
    public bool OtherSafetyInterventions { get; set; }

    public string CmtSafetyInterventions { get; set; } = string.Empty;
    
    public bool ChildOutsideHome { get; set; }
    
    public bool ChildRemoved { get; set; }

    public static SafetyInterventions FromApiEntity(SafetyInterventionsEntity entity)
    {
        return new SafetyInterventions()
        {
            DirectIntervention = entity.DirectIntervention.ParseWordTruthiness(),
            UseOfIndividuals = entity.UseOfIndividuals.ParseWordTruthiness(),
            UseCommAgencies = entity.UseCommAgencies.ParseWordTruthiness(),
            ProtectVictim = entity.ProtectVictim.ParseWordTruthiness(),
            LeaveHome = entity.LeaveHome.ParseWordTruthiness(),
            NonOffendingParent = entity.NonOffendingParent.ParseWordTruthiness(),
            LegalIntPlanned = entity.LegalIntPlanned.ParseWordTruthiness(),
            OtherSafetyInterventions = entity.OtherSafetyInterventions.ParseWordTruthiness(),
            CmtSafetyInterventions = entity.CmtSafetyInterventions,
            ChildOutsideHome = entity.ChildOutsideHome.ParseWordTruthiness(),
            ChildRemoved = entity.ChildRemoved.ParseWordTruthiness(),
        };
    }

    public SafetyInterventionsEntity ToApiEntity()
    {
        return new SafetyInterventionsEntity()
        {
            DirectIntervention = DirectIntervention.AsTruthyChar(),
            UseOfIndividuals = UseOfIndividuals.AsTruthyChar(),
            UseCommAgencies = UseCommAgencies.AsTruthyChar(),
            ProtectVictim = ProtectVictim.AsTruthyChar(),
            LeaveHome = LeaveHome.AsTruthyChar(),
            NonOffendingParent = NonOffendingParent.AsTruthyChar(),
            LegalIntPlanned = LegalIntPlanned.AsTruthyChar(),
            OtherSafetyInterventions = OtherSafetyInterventions.AsTruthyChar(),
            CmtSafetyInterventions = OtherSafetyInterventions ? CmtSafetyInterventions : "",
            ChildOutsideHome = ChildOutsideHome.AsTruthyChar(),
            ChildRemoved = ChildRemoved.AsTruthyChar(),
        };
    }
}
