using Realms;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Models.Notes;

public partial class NoteDraft : IRealmObject, IDraftItem
{
    // Only allow one note draft per parent entity.
    [PrimaryKey]
    public string ParentEntityId { get; set; } = Guid.NewGuid().ToString();

    public string RelatedEntityId
    {
        get => ParentEntityId;
        set { }
    }

    private int RelatedEntityTypeInt { get; set; } = (int)EntityType.Unknown;

    public EntityType RelatedEntityType
    {
        get => (EntityType)RelatedEntityTypeInt;
        set => RelatedEntityTypeInt = (int)value;
    }

    private int RelatedEntitySubtypeInt { get; set; } = (int)EntitySubtype.Unknown;

    public EntitySubtype RelatedEntitySubtype
    {
        get => (EntitySubtype)RelatedEntitySubtypeInt;
        set => RelatedEntitySubtypeInt = (int)value;
    }

    public string Draft { get; set; } = string.Empty;

    public DateTimeOffset DraftCreated { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    public string Preview => GeneralStrings.Note;

    public string DraftLocation { get; set; } = string.Empty;

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

    public static string MakeId(string parentEntityId)
    {
        return $"{parentEntityId}";
    }

    public static NoteDraft? FindByEntityId(Realm realm, string entityId)
    {
        return realm.Find<NoteDraft>(MakeId(entityId));
    }

    public static async Task Delete(Realm realm, string entityNumber)
    {
        var draft = FindByEntityId(realm, entityNumber);

        if (draft != null)
            await realm.WriteAsync(() => realm.Remove(draft));
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
