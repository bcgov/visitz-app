using Realms;

namespace Visitz.Models.SafetyAssess;

public partial class ProtectiveCapacity : IRealmObject
{
	public bool ChildCognitive { get; set; }
	
    public bool ParentCognitive { get; set; }
    
    public bool ParentWillingness { get; set; }
    
    public bool ParentResources { get; set; }
    
    public bool ParentSupportive { get; set; }
    
    public bool ParentProtect { get; set; }
    
    public bool ParentAccept { get; set; }
    
    public bool ParentRelationship { get; set; }
    
    public bool ParentAware { get; set; }
    
    public bool ParentProbSolving { get; set; }
    
    public bool NoProCapPresent { get; set; }
    
    public bool CapacitiesOther { get; set; }
    
    public string CmtProtectiveCapacity01 { get; set; }
    
    public string CmtProtectiveCapacity02 { get; set; }
    
}
