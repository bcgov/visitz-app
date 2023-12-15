using Realms;

namespace Visitz.Models.SafetyAssess;

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
    
    public bool CmtSafetyInterventions { get; set; }
    
    public bool ChildOutsideHome { get; set; }
    
    public bool ChildRemoved { get; set; }
    
}
