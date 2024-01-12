using Realms;
using System.Globalization;
using VisitzApi.Models.SafetyAssess;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyAssessment : IRealmObject
{
    public static readonly string DateFormat = "dd/MM/yyyy";

    [Required]
    public string IncidentNumber { get; set; }

    public string WorkerId { get; set; }

    public string FamilyName { get; set; }

    public DateTimeOffset DateOfAssessment { get; set; } = DateTimeOffset.Now;

    public string Operation { get; set; }

    public FactorInfluence FactorInfluence { get; set; }

    public SafetyFactors SafetyFactors { get; set; }

    public ProtectiveCapacity ProtectiveCapacity { get; set; }

    public SafetyInterventions SafetyInterventions { get; set; }

    public SafetyDecisions SafetyDecisions { get; set; }

    public IList<string> ChildsInOutCare { get; }

    public static SafetyAssessment FromApiEntity(SafetyAssessmentEntity entity)
    {
        var safetyAssessment = new SafetyAssessment()
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
        };

        foreach (var childId in entity.ChildsInOutCare)
            safetyAssessment.ChildsInOutCare.Add(childId.ChildContactId);

        return safetyAssessment;
    }

    public SafetyAssessmentEntity ToApiEntity()
    {
        var safetyAssessmentEntity = new SafetyAssessmentEntity()
        {
            IncidentNumber = IncidentNumber,
            WorkerId = WorkerId,
            FamilyName = FamilyName,
            DateOfAssessment = DateOfAssessment.ToString(DateFormat, CultureInfo.InvariantCulture),
            Operation = Operation,
            FactorInfluence = FactorInfluence.ToApiEntity(),
            SafetyFactors = SafetyFactors.ToApiEntity(),
            ProtectiveCapacity = ProtectiveCapacity.ToApiEntity(),
            SafetyInterventions = SafetyInterventions.ToApiEntity(),
            SafetyDecisions = SafetyDecisions.ToApiEntity(),
        };

        foreach (var childId in ChildsInOutCare)
            safetyAssessmentEntity.AddChildContactId(childId);

        return safetyAssessmentEntity;
    }

    public static SafetyAssessment FindByIncidentNumber(Realm realm, string incidentNumber)
    {
        return realm
            .All<SafetyAssessment>()
            .Where(sa => sa.IncidentNumber.Equals(incidentNumber))
            .FirstOrDefault();
    }

    public static async Task Delete(Realm realm, SafetyAssessment safetyAssessment)
    {
        await realm.WriteAsync(() => realm.Remove(safetyAssessment));
    }

    public async Task Save(Realm realm)
    {
        await realm.WriteAsync(() => realm.Add(this, update: true));
    }
}
