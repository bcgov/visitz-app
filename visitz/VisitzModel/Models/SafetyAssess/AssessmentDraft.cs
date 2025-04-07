using Realms;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.SafetyAssess;

public partial class AssessmentDraft : IRealmObject, IDraftItem
{
    [PrimaryKey]
    public string DraftEntityId { get; set; }

    public string RelatedEntityId { get => DraftEntityId; set { } }

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

    public static IQueryable<AssessmentDraft> GetAllByFileNumber(Realm realm, string fileNumber)
    {
        return realm.All<AssessmentDraft>().Where(d => d.DraftEntityId == fileNumber);
    }

    public static async Task<AssessmentDraft> Upsert(
        Realm realm,
        SafetyAssessment assessment,
        string draftLocation,
        EntityType type = EntityType.Incident,
        EntitySubtype subtype = EntitySubtype.ChildProtection)
    {
        var draft = realm.Find<AssessmentDraft>(assessment.IncidentNumber) ?? new()
        {
            DraftEntityId = assessment.IncidentNumber,
        };

        await realm.WriteAsync(() =>
        {
            if (!assessment.IsManaged)
                realm.Add(assessment);

            draft.DraftLocation = draftLocation;
            draft.RelatedEntityType = type;
            draft.RelatedEntitySubtype = subtype;
            draft.LastUpdated = DateTimeOffset.Now;

            realm.Add(draft, update: true);
        });

        return draft;
    }

    public static async Task TryDeleteAsync(SafetyAssessment assessment)
    {
        if (assessment?.Realm == null)
            return;

        var realm = assessment.Realm;

        await realm.WriteAsync(() =>
        {
            if (realm.Find<AssessmentDraft>(assessment.IncidentNumber) is AssessmentDraft draft)
                realm.Remove(draft);

            if (assessment.IsManaged)
                realm.Remove(assessment);
        });
    }
}
