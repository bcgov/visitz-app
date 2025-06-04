namespace VisitzApi.Models.SafetyAssess;

public class SubmitSafetyAssessmentJson
{
    public string IncidentNumber { get; set; }

    public string WorkerId { get; set; }

    public string FamilyName { get; set; }

    public string DateOfAssessment { get; set; }

    public string Operation { get; set; }

    public SubmitFactorInfluenceJson FactorInfluence { get; set; }

    public SubmitSafetyFactorsJson SafetyFactors { get; set; }

    public SubmitProtectiveCapacityJson ProtectiveCapacity { get; set; }

    public SubmitSafetyInterventionsJson SafetyInterventions { get; set; }

    public SubmitSafetyDecisionsJson SafetyDecisions { get; set; }

    public IList<ChildId> ChildsInOutCare { get; set; } = new List<ChildId>();

    public class ChildId
    {
        public string ChildContactId { get; set; }

        public override string ToString() => ChildContactId;
    }

    public void AddChildContactId(string childContactid)
    {
        ChildsInOutCare.Add(new ChildId { ChildContactId = childContactid });
    }
}
