using System.Globalization;
using Realms;
using VisitzApi.Models;
using VisitzModel.Formats;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Interfaces;

namespace VisitzModel.Models.Notes;

/// <summary>
/// The business object that would be used by the app source.
/// </summary>
public partial class NoteItem : IRealmObject, IParentRecord
{
    private const string NotePeriodName = "NotePeriod";
    private const string CreatedDateName = "CreatedDate";

    private static readonly string IcmNotePeriodDateFormat = IcmDateFormats.NotePeriod;
    private static readonly string NoteWrapperTimestampFormat = IcmDateFormats.BasicTimestamp;
    private static readonly string Separator = "────";

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

    /// <summary>
    /// Used app-only to associate Notes with parent records. As of 2023-06-05 the ICM API does
    /// not return PK/FK information about notes.
    /// </summary>
    [Indexed]
    [MapTo("IcmId")]
    public string ParentFileNumber { get; set; } = string.Empty;

    public string ParentId { get; set; } = string.Empty;

    private int ParentTypeInt { get; set; }
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
            NotePeriodField = value;

            NotePeriodDateTime = value?.Length > 0 ? DateTimeOffset.Parse(value) : DateTimeOffset.MinValue;
        }
    }

    [MapTo(CreatedDateName)]
    private string CreatedDateField { get; set; } = string.Empty;
    public string CreatedDate
    {
        get => CreatedDateField;
        set
        {
            CreatedDateField = value;

            CreatedDateTime = value?.Length > 0 ? DateTimeOffset.Parse(value) : DateTimeOffset.MinValue;
        }
    }

    public string Content { get; set; } = string.Empty;
    public int PageNumber { get; set; }

    public DateTimeOffset NotePeriodDateTime { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }

    public string PeriodOrPageNumber => NotePeriod?.Length > 0 ? NotePeriod : $"Page {PageNumber}";

    public static string MakeFullID(string icmId, NoteEntity note)
    {
        return MakeFullID(icmId, note.NotePeriod, note.CreatedDate);
    }

    public static string MakeFullID(string icmId, string notePeriod, string createdDate)
    {
        return $"{icmId}-{notePeriod}-{createdDate}";
    }

    public static NoteItem FromApiEntity(
        string parentFileNumber,
        EntityType parentType,
        NoteEntity note,
        int pageNumber
    )
    {
        return new NoteItem()
        {
            FullID = MakeFullID(parentFileNumber, note),
            ParentType = parentType,
            ParentFileNumber = parentFileNumber,
            NotePeriod = note.NotePeriod,
            CreatedDate = note.CreatedDate,
            Content = note.Content,
            PageNumber = pageNumber,
        };
    }

    public static IEnumerable<NoteItem> FromApiEntities(
        string parentFileNumber,
        EntityType parentType,
        IEnumerable<NoteEntity> noteEntities
    )
    {
        return noteEntities
            .OrderBy(item => NoteEntity.NotePeriodDateTimeTransform(item, true))
            .ThenBy(item => NoteEntity.CreatedDateTimeTransform(item, true))
            .Select((note, index) => FromApiEntity(parentFileNumber, parentType, note, index + 1));
    }

    public static async Task UpsertNotesAsync(
        Realm realm,
        string parentFileNumber,
        EntityType parentEntityType,
        IEnumerable<NoteItem> newNotes
    )
    {
        if (parentEntityType == EntityType.Case)
            // Case notes older <= 2012 may have a blank note period.
            newNotes = SimulateNotePeriods(newNotes);

        var currentNotes = GetNotesByFileNumber(realm, parentFileNumber);
        var deletedNotes = currentNotes.ExceptBy(newNotes.Select(NoteSelector), NoteSelector);

        await realm.WriteAsync(() =>
        {
            foreach (var deletedNote in deletedNotes)
                realm.Remove(deletedNote);

            realm.Add(newNotes, update: true);
        });
    }

    static List<NoteItem> SimulateNotePeriods(IEnumerable<NoteItem> notes)
    {
        var simulatedPeriodNotes = notes.ToList();

        foreach (var note in simulatedPeriodNotes)
            if (string.IsNullOrWhiteSpace(note.NotePeriod))
                note.NotePeriod = NotePeriodFrom(DateTimeOffset.Parse(note.CreatedDate));

        return simulatedPeriodNotes;
    }

    static string NoteSelector(NoteItem note) => note.FullID;

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

    public static NoteItem? GetLatestByEntityId(Realm realm, string parentFileNumber)
    {
        return GetNotesByFileNumber(realm, parentFileNumber).LastOrDefault();
    }

    public static IQueryable<NoteItem> GetNotesByFileNumber(Realm realm, string parentFileNumber)
    {
        return realm
            .All<NoteItem>()
            .Where(item => item.ParentFileNumber == parentFileNumber)
            .Filter($"TRUEPREDICATE SORT({nameof(NotePeriodDateTime)} ASC, {nameof(CreatedDateTime)} ASC)");
    }

    public static void RemoveByParentFileNumber(Realm realm, EntityType type, string fileNumber)
    {
        var noteItems = realm
            .All<NoteItem>()
            .Where(item => item.ParentFileNumber == fileNumber && item.ParentTypeInt == (int)type);

        realm.RemoveRange(noteItems);
    }
}
