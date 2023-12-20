namespace VisitzApi.Models.SafetyAssess;

public class SafetyAssessmentEntity
{
    public string IncidentNumber { get; set; }

    public string WorkerId { get; set; }

    public string FamilyName { get; set; }

    public string DateOfAssessment { get; set; }

    public string Operation { get; set; }

    public FactorInfluenceEntity FactorInfluence { get; set; }

    public SafetyFactorsEntity SafetyFactors { get; set; }

    public ProtectiveCapacityEntity ProtectiveCapacity { get; set; }

    public SafetyInterventionsEntity SafetyInterventions { get; set; }

    public SafetyDecisionsEntity SafetyDecisions { get; set; }

    public IList<string> ChildsInOutCare { get; }
}
