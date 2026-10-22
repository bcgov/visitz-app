using Realms;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.InPersonVisits;

public partial class PersonVisitDraft : IRealmObject, IDraftItem
{
    [PrimaryKey]
    public string RelatedEntityId { get; set; } = Guid.NewGuid().ToString();

    internal int RelatedEntityTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    internal int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.ChildServices;

    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    public string Preview => GeneralStrings.Visit;

    public string DraftLocation { get; set; } = string.Empty;

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public PersonVisit? Visit { get; set; } = new();

    public PersonVisitDraft() { }

    public PersonVisitDraft(CaseRecord @case)
    {
        RelatedEntityId = @case.Id;
        DraftLocation = @case.Name;
    }

    public static PersonVisitDraft? GetDraft(Realm realm, string caseId)
    {
        return realm.Find<PersonVisitDraft>(caseId);
    }

    public static async Task<PersonVisitDraft> Upsert(
        Realm realm,
        string caseId,
        PersonVisit visit,
        string draftLocation
    )
    {
        var draft =
            realm.Find<PersonVisitDraft>(caseId)
            ?? new()
            {
                RelatedEntityId = caseId,
                DraftLocation = draftLocation,
                Visit = visit ?? new(),
            };

        if (draft.Visit == null)
            throw new InvalidOperationException("Draft's visit was null");

        draft.Visit.ParentId = caseId;

        await RealmExtensions.CommitAsync(realm, () => realm.Upsert(draft));

        return draft;
    }
}
