using Realms;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyAssessment : IRealmObject
{
	public string IncidentNumber { get; set; }
	
    public string WorkerId { get; set; }
    
    public string FamilyName { get; set; }
    
    public string DateOfAssessment { get; set; }
    
    public string Operation { get; set; }
    
    public FactorInfluence FactorInfluence { get; set; }

    public SafetyFactors SafetyFactors { get; set; }

    public ProtectiveCapacity ProtectiveCapacity { get; set; }

    public SafetyInterventions SafetyInterventions { get; set; }

    public SafetyDecisions SafetyDecisions { get; set; }

    public IList<string> ChildsInOutCare { get; }
}
