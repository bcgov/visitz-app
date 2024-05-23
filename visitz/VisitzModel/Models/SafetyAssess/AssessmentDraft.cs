using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.SafetyAssess;
public partial class AssessmentDraft : IRealmObject, IDraftItem
{
	[PrimaryKey]
	public string DraftEntityId { get; set; }

	public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;
	public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

	public string Preview => GeneralStrings.SafetyAssessment;

	public string DraftLocation { get; set; }

	int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;
	public EntityType RelatedEntityType
	{
		get => (EntityType)RelatedEntityTypeInt;
		set => RelatedEntityTypeInt = (int)value;
	}

	int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;
	public EntitySubtype RelatedEntitySubtype
	{
		get => (EntitySubtype)RelatedEntitySubtypeInt;
		set => RelatedEntitySubtypeInt = (int)value;
	}

	public static async Task<AssessmentDraft> Upsert(
		SafetyAssessment assessment,
		string draftLocation,
		EntityType type = EntityType.Incident,
		EntitySubtype subtype = EntitySubtype.ChildProtection)
	{
		var realm = assessment.Realm;

		var draft = realm.Find<AssessmentDraft>(assessment.IncidentNumber) ?? new()
		{
			DraftEntityId = assessment.IncidentNumber,
		};

		await realm.WriteAsync(async () =>
		{
			await SafetyAssessment.Save(realm, assessment);

			draft.DraftLocation = draftLocation;
			draft.RelatedEntityType = type;
			draft.RelatedEntitySubtype = subtype;
			draft.LastUpdated = DateTimeOffset.Now;

			realm.Add(draft, update: true);
		});

		return draft;
	}
}
