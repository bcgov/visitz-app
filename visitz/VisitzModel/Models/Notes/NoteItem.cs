using System.Globalization;
using Realms;
using VisitzApi.Models.Notes;
using VisitzModel.Formats;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.Notes;

/// <summary>
/// The business object that would be used by the app source.
/// </summary>
public partial class NoteItem : IRealmObject, IParentRecord, IEquatable<NoteItem>
{
    private const string NotePeriodName = "NotePeriod";
    private const string NotePeriodDateTimeName = "NotePeriodDateTime";

    private static readonly string IcmNotePeriodDateFormat = IcmDateFormats.NotePeriod;
    private static readonly string NoteWrapperTimestampFormat = IcmDateFormats.BasicTimestamp;
    private static readonly string Separator = "────";

    private static readonly IComparer<NoteItem> _fullIdComparer = Comparer<NoteItem>.Create(
        (l, r) => l.FullID.CompareTo(r.FullID)
    );

    /// <summary>
    /// Used app-only to uniquely ID NoteItems.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The ICM API doesn't provide a NoteItem's actual primary key ID, so this value is made using <c>IcmId</c>,
    /// <c>NotePeriod</c>, and <c>CreatedDate</c>.
    /// </para>
    /// <para>
    /// <i>This may become an issue</i>—if two note objects are created in the same second for a given ICM entity, this PK
    /// unique assertion will fail. While possible, this is also unlikely: new note objects are typically only
    /// created either monthly or when a ~16000 character limit is reached.
    /// </para>
    /// </remarks>
    [PrimaryKey]
    public string FullID { get; set; } = string.Empty;

    [Indexed]
    public string ParentId { get; set; } = string.Empty;

    [Indexed]
    internal int ParentTypeInt { get; set; }
    public EntityType ParentType
    {
        get => (EntityType)ParentTypeInt;
        set => ParentTypeInt = (int)value;
    }

    [MapTo(NotePeriodName)]
    private string NotePeriodField { get; set; } = string.Empty;
    public string NotePeriod
    {
        get => NotePeriodField;
        set
        {
            NotePeriodDateTimeField = Timestamp.ParseDateTimeOffsetNullable(value, CultureInfo.InvariantCulture);
            NotePeriodField = value;
        }
    }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.MinValue;

    public string Content { get; set; } = string.Empty;
    public int PageNumber { get; set; }

    [MapTo(NotePeriodDateTimeName)]
    private DateTimeOffset? NotePeriodDateTimeField { get; set; }
    public DateTimeOffset NotePeriodDateTime
    {
        get =>
            DateTimeOffset.TryParse(NotePeriodField, out DateTimeOffset dateTime) ? dateTime : DateTimeOffset.MinValue;
        set
        {
            NotePeriodField = value.ToString(IcmNotePeriodDateFormat, CultureInfo.InvariantCulture);
            NotePeriodDateTimeField = value;
        }
    }

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedByName { get; set; } = string.Empty;

    public string CreatedByOffice { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedByName { get; set; } = string.Empty;

    public string PeriodOrPageNumber => NotePeriod?.Length > 0 ? NotePeriod : $"Page {PageNumber}";

    public NoteItem() { }

    NoteItem(INoteJson note)
    {
        CreatedDate = note.Created;
        CreatedBy = note.CreatedBy;
        CreatedByName = note.CreatedByName;
        CreatedByOffice = note.CreatedByOffice;
        FullID = note.Id;
        Content = note.Text;
        UpdatedDate = note.Updated;
        UpdatedBy = note.UpdatedBy;
        UpdatedByName = note.UpdatedByName;
    }

    public NoteItem(CaseNoteJson caseNote, string parentId)
        : this(caseNote)
    {
        NotePeriod = caseNote.NotePeriod;
        ParentId = parentId;
        ParentType = EntityType.Case;
    }

    public NoteItem(ResponseNarrativeJson narrativeJson, EntityType parentType, string parentId)
        : this(narrativeJson)
    {
        if (!(parentType is EntityType.Incident or EntityType.ServiceRequest))
            throw new InvalidOperationException($"Type '{parentType}' not allowed");

        ParentId = parentId;
        ParentType = parentType;
    }

    public static NoteItem FromApiEntity(string parentId, EntityType parentType, INoteJson noteJson, int pageNumber)
    {
        if (!(parentType is EntityType.Case or EntityType.Incident or EntityType.ServiceRequest))
            throw new InvalidOperationException($"Type '{parentType}' not allowed");

        NoteItem note = new(noteJson)
        {
            ParentType = parentType,
            ParentId = parentId,
            PageNumber = pageNumber,
        };

        if (noteJson is CaseNoteJson caseNote)
            note.NotePeriod = caseNote.NotePeriod;

        return note;
    }

    public static IEnumerable<NoteItem> FromApiEntities(string parentId, IEnumerable<CaseNoteJson> noteEntities)
    {
        // Case notes do not need to be sorted for page numbers like response narrative notes do
        return noteEntities.Select(note => FromApiEntity(parentId, EntityType.Case, note, 0));
    }

    public static IList<NoteItem> FromApiEntities(
        EntityType parentType,
        string parentId,
        IEnumerable<ResponseNarrativeJson> noteEntities
    )
    {
        return noteEntities
            .OrderBy(note => note.Created)
            .Select((note, index) => FromApiEntity(parentId, parentType, note, index + 1))
            .ToList();
    }

    public static async Task SynchronizeAsync(
        Realm realm,
        string parentId,
        EntityType parentType,
        IEnumerable<NoteItem> incomingNotes
    )
    {
        if (parentType == EntityType.Case)
            // Case notes older <= 2012 may have a blank note period.
            incomingNotes = SimulateNotePeriods(incomingNotes);

        var currentNotes = GetNotesByParent(realm, parentType, parentId).ToList().Order(_fullIdComparer).ToList();

        // ToList required because of Realm object lifecycles
        var updateNotes = incomingNotes
            .Intersect(currentNotes)
            .Where(incoming => ShouldUpdate(currentNotes, incoming))
            .ToList();
        var deletedNotes = currentNotes.Except(incomingNotes).ToList();
        var insertNotes = incomingNotes.Except(currentNotes).ToList();

        await realm.WriteAsync(() =>
        {
            foreach (var deletedNote in deletedNotes)
                realm.Remove(deletedNote);

            foreach (var insertNote in insertNotes)
                realm.Add(insertNote);

            foreach (var updateNote in updateNotes)
                realm.Add(updateNote, update: true);
        });
    }

    static bool ShouldUpdate(List<NoteItem> currentNotes, NoteItem updateNote)
    {
        int index = currentNotes.BinarySearch(updateNote, _fullIdComparer);
        if (index < 0)
            // updateNote not in currentNotes, fail early. If it's not here, it should've been added from insertNotes.
            return false;

        NoteItem currentNote = currentNotes[index];
        return !currentNote.DeepEquals(updateNote);
    }

    static List<NoteItem> SimulateNotePeriods(IEnumerable<NoteItem> notes)
    {
        var simulatedPeriodNotes = notes.ToList();

        foreach (var note in simulatedPeriodNotes)
            if (string.IsNullOrWhiteSpace(note.NotePeriod))
                note.NotePeriod = NotePeriodFrom(note.CreatedDate);

        return simulatedPeriodNotes;
    }

    public bool DeepEquals(NoteItem other)
    {
        return Equals(other)
            && ParentType == other.ParentType
            && NotePeriod == other.NotePeriod
            && CreatedDate == other.CreatedDate
            && CreatedBy == other.CreatedBy
            && UpdatedDate == other.UpdatedDate
            && UpdatedBy == other.UpdatedBy
            && Content == other.Content
            && PageNumber == other.PageNumber
            && NotePeriodDateTime == other.NotePeriodDateTime;
    }

    public bool Equals(NoteItem? other)
    {
        return ReferenceEquals(this, other) || FullID == other?.FullID;
    }

    public override bool Equals(object? other)
    {
        return other is NoteItem note && Equals(note) || base.Equals(other);
    }

    public override int GetHashCode()
    {
#pragma warning disable SS008 // GetHashCode() refers to mutable or static member
        return FullID.GetHashCode();
#pragma warning restore SS008 // GetHashCode() refers to mutable or static member
    }

    public static string NotePeriodFrom(DateTime dateTime)
    {
        return NotePeriodFrom(new DateTimeOffset(dateTime));
    }

    public static string NotePeriodFrom(DateTimeOffset dateTime)
    {
        return dateTime.ToString(IcmNotePeriodDateFormat, CultureInfo.InvariantCulture);
    }

    public static string WrapContent(string idir, DateTime dateTime, string content)
    {
        var timestamp = dateTime.ToString(NoteWrapperTimestampFormat, CultureInfo.InvariantCulture);
        return $"{Separator} {idir} {timestamp} {Separator}\n{content}";
    }

    public static NoteItem? GetLatestByEntityId(Realm realm, EntityType parentType, string parentId)
    {
        return GetNotesByParent(realm, parentType, parentId).LastOrDefault();
    }

    public static IQueryable<NoteItem> GetNotesByParent(Realm realm, EntityType parentType, string parentId)
    {
        return realm
            .All<NoteItem>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)parentType)
            .Filter($"TRUEPREDICATE SORT({nameof(NotePeriodDateTime)} ASC, {nameof(CreatedDate)} ASC)");
    }

    public static void RemoveByParent(Realm realm, EntityType parentType, string parentId)
    {
        var noteItems = realm
            .All<NoteItem>()
            .Where(item => item.ParentId == parentId && item.ParentTypeInt == (int)parentType);

        realm.RemoveRange(noteItems);
    }
}
