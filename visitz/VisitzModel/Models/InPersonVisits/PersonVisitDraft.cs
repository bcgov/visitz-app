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

    private int RelatedEntityTypeInt { get; set; } = (int)EntityType.Case;

    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    private int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.ChildServices;

    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    public string Preview => string.Format(GeneralStrings.VisitDate, Visit?.DateOfVisit.ToString("D"));

    public string DraftLocation { get; set; } = string.Empty;

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public PersonVisit? Visit { get; set; } = new();

    private bool disposedValue;
    private bool? relatedEntityAvailable;
    private bool? relatedEntityDownloaded;

    [Ignored]
    public Realm? RelatedEntityRealm { get; set; }

    [Ignored]
    public IQueryable<IBusinessObject>? RelatedEntitySubscriptionQuery { get; set; }

    [Ignored]
    public IDisposable? RelatedEntitySubscriptionToken { get; set; }

    /// <summary>
    /// Whether or not the related entity is available for the app to interact
    /// with at all.
    /// </summary>
    [Ignored]
    public bool? RelatedEntityAvailable
    {
        get => relatedEntityAvailable;
        set
        {
            relatedEntityAvailable = value;
            RaisePropertyChanged(nameof(RelatedEntityAvailable));
        }
    }

    /// <summary>
    /// Whether or not the related entity's depdendent data has been
    /// downloaded (or marked for download).
    /// </summary>
    [Ignored]
    public bool? RelatedEntityDownloaded
    {
        get => relatedEntityDownloaded;
        set
        {
            relatedEntityDownloaded = value;
            RaisePropertyChanged(nameof(RelatedEntityDownloaded));
        }
    }

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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                RelatedEntitySubscriptionToken?.Dispose();
                RelatedEntitySubscriptionToken = null;
                RelatedEntitySubscriptionQuery = null;
                RelatedEntityRealm = null;
                RelatedEntityAvailable = null;
                RelatedEntityDownloaded = null;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public int CompareTo(IDraftItem? other)
    {
        return this.CompareDraftItem(other);
    }
}
