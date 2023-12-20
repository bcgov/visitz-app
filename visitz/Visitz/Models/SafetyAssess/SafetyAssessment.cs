using Realms;
using VisitzApi.Models.SafetyAssess;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyAssessment : IRealmObject
{
    [Required]
    public string IncidentNumber { get; set; }

    public string WorkerId { get; set; }

    public string FamilyName { get; set; }

    public DateTimeOffset DateOfAssessment { get; set; }

    public string Operation { get; set; }

    public FactorInfluence FactorInfluence { get; set; }

    public SafetyFactors SafetyFactors { get; set; }

    public ProtectiveCapacity ProtectiveCapacity { get; set; }

    public SafetyInterventions SafetyInterventions { get; set; }

    public SafetyDecisions SafetyDecisions { get; set; }

    public IList<string> ChildsInOutCare { get; }

    public static SafetyAssessment FromApiEntity(SafetyAssessmentEntity entity)
    {
        return new SafetyAssessment()
        {
            IncidentNumber = entity.IncidentNumber,
            WorkerId = entity.WorkerId,
            FamilyName = entity.FamilyName,
            DateOfAssessment = DateTimeOffset.Parse(entity.DateOfAssessment),
            Operation = entity.Operation,
            FactorInfluence = FactorInfluence.FromApiEntity(entity.FactorInfluence),
            SafetyFactors = SafetyFactors.FromApiEntity(entity.SafetyFactors),
            ProtectiveCapacity = ProtectiveCapacity.FromApiEntity(entity.ProtectiveCapacity),
            SafetyInterventions = SafetyInterventions.FromApiEntity(entity.SafetyInterventions),
            SafetyDecisions = SafetyDecisions.FromApiEntity(entity.SafetyDecisions),
            //ChildsInOutCare = entity.ChildsInOutCare, TODO: implement
        };
    }

    public SafetyAssessmentEntity ToApiEntity()
    {
        return new SafetyAssessmentEntity()
        {
            IncidentNumber = IncidentNumber,
            WorkerId = WorkerId,
            FamilyName = FamilyName,
            DateOfAssessment = DateOfAssessment.ToString(IcmDateFormat.Format),
            Operation = Operation,
            FactorInfluence = FactorInfluence.ToApiEntity(),
            SafetyFactors = SafetyFactors.ToApiEntity(),
            ProtectiveCapacity = ProtectiveCapacity.ToApiEntity(),
            SafetyInterventions = SafetyInterventions.ToApiEntity(),
            SafetyDecisions = SafetyDecisions.ToApiEntity(),
            //ChildsInOutCare = ChildsInOutCare, TODO: implement
        };
    }
}
