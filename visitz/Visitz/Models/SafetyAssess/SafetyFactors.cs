using Realms;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyFactors : IRealmObject
{
    public bool PhysicalHarm { get; set; }
        
    public bool SeriousInjuryAbuse { get; set; }
        
    public bool FearsMaltreatChild { get; set; }
        
    public bool ThreatAgainstChild { get; set; }
        
    public bool ExcessiveForce { get; set; }
        
    public bool SubsExposedInfant { get; set; }
        
    public string CmtClarification { get; set; }
        
    public bool CurrentCircumstances { get; set; }
        
    public string CmtCircumstances { get; set; }
        
    public bool SexAbuse { get; set; }
        
    public string CmtAbuse { get; set; }
        
    public bool UnableToProtect { get; set; }
        
    public string CmtProtect { get; set; }
        
    public bool InjuryExplanation { get; set; }
        
    public string CmtExplanation { get; set; }
        
    public bool RefuseAccess { get; set; }
        
    public string CmtAccess { get; set; }
        
    public bool ImmediateNeeds { get; set; }
        
    public string CmtNeeds { get; set; }
        
    public bool PhysicalCondition { get; set; }
        
    public string CmtCondition { get; set; }
        
    public bool CurrentAbuse { get; set; }
        
    public string CmtCurrent { get; set; }
        
    public bool PartnerViolence { get; set; }
        
    public string CmtViolence { get; set; }
        
    public bool PredominantlyNegative { get; set; }
        
    public string CmtNegative { get; set; }
        
    public bool EmotionalStability { get; set; }
        
    public string CmtEmotional { get; set; }
        
    public bool ChildFearful { get; set; }
        
    public string CmtFearful { get; set; }
        
    public bool OtherFactors { get; set; }
        
    public string CmtOtherFactors { get; set; }
        
    public bool CurretAbuse { get; set; }
        
}
