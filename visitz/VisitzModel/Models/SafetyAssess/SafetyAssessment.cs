using Realms;
using System.Globalization;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyAssessment : IRealmObject, IRowMetadata, IApiJson<SubmitSafetyAssessmentJson>
{
    public static readonly string DateFormat = "dd/MM/yyyy";
    public static readonly int CommentsMaxLength = 1000;

    [PrimaryKey]
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset UpdatedDate { get; set; }

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

    public string ApprovedBy { get; set; } = string.Empty;

    public string ApprovedDate { get; set; } = string.Empty;

    public string ApprovedToFinalize { get; set; } = string.Empty;

    public DateTimeOffset? ApprovedToFinalizeDate { get; set; }

    public DateTimeOffset? FinalizedDate { get; set; }

    public string ApprovedToFinalizeDS { get; set; } = string.Empty;

    public string DataStewardRole { get; set; } = string.Empty;

    public string SocialWorkerFirstName { get; set; } = string.Empty;

    public string SocialWorkerId { get; set; } = string.Empty;

    public string SocialWorkerLastName { get; set; } = string.Empty;

    public string TeamLeaderFirstName { get; set; } = string.Empty;

    public string TeamLeaderId { get; set; } = string.Empty;

    public string TeamLeaderLastName { get; set; } = string.Empty;

    public string TeamLeaderLoginName { get; set; } = string.Empty;

    public string Type {  get; set; } = string.Empty;

    public static SafetyAssessment FromApiJson(string fileNumber, SafetyAsessmentJson json)
    {
        var safetyAssessment = new SafetyAssessment()
        {
            Id = json.Id,
            CreatedBy = json.CreatedBy,
            CreatedById = json.CreatedById,
            CreatedDate = DateTimeOffset.Parse(json.CreatedDate),
            UpdatedBy = json.UpdatedBy,
            UpdatedById = json.UpdatedById,
            UpdatedDate = DateTimeOffset.Parse(json.UpdatedDate),
            IncidentNumber = fileNumber,
            WorkerId = json.CreatedBy,
            FamilyName = json.FamilyName,
            DateOfAssessment = DateTimeOffset.Parse(json.DateOfAssessment),
            Operation = "",
            ApprovedBy = json.ApprovedBy,
            ApprovedDate = json.ApprovedDate,
            ApprovedToFinalize = json.ApprovedToFinalize,
            ApprovedToFinalizeDate = Timestamp.ParseDateTimeOffsetNullable(json.ApprovedToFinalizeDate),
            FinalizedDate = Timestamp.ParseDateTimeOffsetNullable(json.FinalizedDate),
            ApprovedToFinalizeDS = json.ApprovedToFinalizeDS,
            DataStewardRole = json.DataStewardRole,
            SocialWorkerFirstName = json.SocialWorkerFirstName,
            SocialWorkerId = json.SocialWorkerId,
            SocialWorkerLastName = json.SocialWorkerLastName,
            TeamLeaderFirstName = json.TeamLeaderFirstName,
            TeamLeaderId = json.TeamLeaderId,
            TeamLeaderLastName = json.TeamLeaderLastName,
            TeamLeaderLoginName = json.TeamLeaderLoginName,
            Type = json.Type,
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
        List<SafetyAssessment> assessments = [];

        foreach (var assessment in json)
            assessments.Add(FromApiJson(incidentId, assessment));

        return assessments;
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

    public static IQueryable<SafetyAssessment> GetAllByFileNumber(Realm realm, string fileNumber)
    {
        return realm.All<SafetyAssessment>().Where(a => a.IncidentNumber == fileNumber);
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

    public static async Task Save(Realm realm, string fileNumber, IEnumerable<SafetyAssessment> assessments)
    {
        var newIds = assessments.Select(a => a.Id);

        var idsToRemove = GetAllByFileNumber(realm, fileNumber)
            .AsEnumerable()
            .Select(a => a.Id)
            .Except(newIds);

        await realm.CommitAsync(() =>
        {
            realm.DeleteByIds<SafetyAssessment>(idsToRemove);
            realm.Upsert(assessments);
        });
    }

    public async Task Save(Realm realm)
    {
		await Save(realm, this);
    }
}
