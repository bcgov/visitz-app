using Realms;
using System.Globalization;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyAssessment : IRealmObject, IApiJson<SubmitSafetyAssessmentJson>
{
    public static readonly string DateFormat = "dd/MM/yyyy";
    public static readonly int CommentsMaxLength = 1000;

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

    public static SafetyAssessment FromApiJson(string fileNumber, SafetyAsessmentJson json)
    {
        var safetyAssessment = new SafetyAssessment()
        {
            IncidentNumber = fileNumber,
            WorkerId = json.CreatedBy,
            FamilyName = json.FamilyName,
            DateOfAssessment = DateTimeOffset.Parse(json.DateOfAssessment),
            Operation = "",
            FactorInfluence = FactorInfluence.FromApiJson(json),
            SafetyFactors = SafetyFactors.FromApiJson(json),
            ProtectiveCapacity = ProtectiveCapacity.FromApiJson(json),
            SafetyInterventions = SafetyInterventions.FromApiJson(json),
            SafetyDecisions = SafetyDecisions.FromApiJson(json),
        };

        foreach (var contact in json.ContactsInOutCare)
            safetyAssessment.ChildsInOutCare.Add(contact.Id);

        return safetyAssessment;
    }

    public static IEnumerable<SafetyAssessment> FromApiJson(
        string incidentId,
        IEnumerable<SafetyAsessmentJson> json)
    {
        return json.Select(j => FromApiJson(incidentId, j));
    }

    public SubmitSafetyAssessmentJson ToApiJson(string dateFormat = "s")
    {
        var safetyAssessmentEntity = new SubmitSafetyAssessmentJson()
        {
            IncidentNumber = IncidentNumber,
            WorkerId = WorkerId,
            FamilyName = FamilyName,
            DateOfAssessment = DateOfAssessment.ToString(DateFormat, CultureInfo.InvariantCulture),
            Operation = Operation,
            FactorInfluence = FactorInfluence.ToApiJson(dateFormat),
            SafetyFactors = SafetyFactors.ToApiJson(dateFormat),
            ProtectiveCapacity = ProtectiveCapacity.ToApiJson(dateFormat),
            SafetyInterventions = SafetyInterventions.ToApiJson(dateFormat),
            SafetyDecisions = SafetyDecisions.ToApiJson(dateFormat),
        };

        if (ChildsInOutCare.Count == 0)
            safetyAssessmentEntity.AddChildContactId("");
        else
            foreach (var childId in ChildsInOutCare)
                safetyAssessmentEntity.AddChildContactId(childId);

        return safetyAssessmentEntity;
    }

    public static SafetyAssessment FindByIncidentNumber(Realm realm, string incidentNumber)
    {
        bool query(SafetyAssessment sa) => sa.IncidentNumber.Equals(incidentNumber);

        // Running the FirstOrDefault() query without checking Any() first sometimes leads to an uncatchable
        // ArgumentOutOfRangeException. I wasn't able to track down the root cause of it, so this workaround
        // will have to do for now.
        //
        // https://github.com/realm/realm-dotnet/issues/3090#issuecomment-1313661344
        // https://github.com/realm/realm-dotnet/issues/3092
        // https://github.com/realm/realm-dotnet/issues/3333

        return realm.All<SafetyAssessment>().Any(query)
            ? realm.All<SafetyAssessment>().Where(query).FirstOrDefault()
            : null;
    }

    public static async Task Delete(Realm realm, SafetyAssessment safetyAssessment)
    {
        await realm.WriteAsync(() => realm.Remove(safetyAssessment));
    }

	public static async Task Save(Realm realm, SafetyAssessment assessment)
	{
		if (!assessment.IsManaged)
		{
			if (realm.IsInTransaction)
				realm.Add(assessment);
			else
				await realm.WriteAsync(() => realm.Add(assessment));
		}
	}

    public async Task Save(Realm realm)
    {
		await Save(realm, this);
    }
}
