using System.Collections.ObjectModel;
using Realms;
using VisitzModel.Extensions;
using VisitzModel.Models.Comparers;
using VisitzModel.Models.EntityTypes;

namespace VisitzModel.Models.Notes;

public partial class NoteItemGroup : ObservableCollection<NoteItem>
{
    private const string DistinctQuery = "TRUEPREDICATE DISTINCT({0})";

    private string NotePageNumberHeaderTemplate { get; set; }

    public string Name =>
        EntityType == EntityType.Case
            ? NoteItem.NotePeriodFrom(NotePeriodDateTime)
            : MakePageNumberHeader(NotePageNumberHeaderTemplate, PageNumber);

    public DateTimeOffset NotePeriodDateTime { get; private set; }

    public int PageNumber { get; private set; }

    public EntityType EntityType { get; private set; }

    public NoteItemGroup(List<NoteItem> notes, EntityType entityType, string notePageNumberHeaderTemplate)
        : base(notes)
    {
        var note = notes.First();

        NotePeriodDateTime = note.NotePeriodDateTime;
        NotePageNumberHeaderTemplate = notePageNumberHeaderTemplate;
        PageNumber = note.PageNumber;
        EntityType = entityType;
    }

    private static string MakePageNumberHeader(string notePageNumberHeaderTemplate, int pageNumber)
    {
        return notePageNumberHeaderTemplate.Format(pageNumber);
    }

    private static IOrderedEnumerable<string> GetPeriodHeaders(IQueryable<NoteItem> entityNotesQuery)
    {
        return entityNotesQuery
            .Filter(DistinctQuery.Format(nameof(NoteItem.NotePeriod)))
            .AsEnumerable()
            .Select(item => item.NotePeriod)
            .OrderBy(DateTime.Parse);
    }

    private static IOrderedEnumerable<int> GetPageHeaders(IQueryable<NoteItem> entityNotesQuery)
    {
        return entityNotesQuery
            .Filter(DistinctQuery.Format(nameof(NoteItem.PageNumber)))
            .AsEnumerable()
            .Select(item => item.PageNumber)
            .Order();
    }

    private static NoteItemGroup GetNotesGroupByPeriod(string notePeriod, IQueryable<NoteItem> entityNotesQuery)
    {
        var notesForPeriod = entityNotesQuery
            .Where(item => item.NotePeriod == notePeriod)
            .AsEnumerable()
            .OrderBy(item => item.CreatedDate)
            .ToList();

        return new NoteItemGroup(notesForPeriod, EntityType.Case, string.Empty);
    }

    private static NoteItemGroup GetNotesGroupByPage(
        int pageNumber,
        IQueryable<NoteItem> entityNotesQuery,
        string notePageNumberHeaderTemplate
    )
    {
        var notesForPage = entityNotesQuery
            .Where(item => item.PageNumber == pageNumber)
            .AsEnumerable()
            .OrderBy(item => item.CreatedDate)
            .ToList();

        string groupName = MakePageNumberHeader(notePageNumberHeaderTemplate, pageNumber);
        return new NoteItemGroup(notesForPage, EntityType.Incident, notePageNumberHeaderTemplate);
    }

    public static List<NoteItemGroup> GetGroupsFromNotesQuery(
        EntityType icmEntityType,
        IQueryable<NoteItem> entityNotesQuery,
        string notePageNumberHeaderTemplate
    )
    {
        var groups = new List<NoteItemGroup>();

        if (icmEntityType == EntityType.Case)
        {
            foreach (var notePeriod in GetPeriodHeaders(entityNotesQuery))
                groups.Add(GetNotesGroupByPeriod(notePeriod, entityNotesQuery));
        }
        else
        {
            foreach (var pageNumber in GetPageHeaders(entityNotesQuery))
                groups.Add(GetNotesGroupByPage(pageNumber, entityNotesQuery, notePageNumberHeaderTemplate));
        }

        return groups;
    }

    private static NoteItemGroup? GetLastTargetGroup(
        IList<NoteItemGroup> groups,
        NoteItem note,
        EntityType entityType,
        string notePageNumberHeaderTemplate = ""
    )
    {
        return entityType == EntityType.Case
            ? groups.LastOrDefault(group => group.Name == note.NotePeriod)
            : groups.LastOrDefault(group =>
                group.Name == MakePageNumberHeader(notePageNumberHeaderTemplate, note.PageNumber)
            );
    }

    public static void InsertInSortedGroups(
        ObservableCollection<NoteItemGroup> groups,
        NoteItem note,
        EntityType entityType,
        string notePageNumberHeaderTemplate
    )
    {
        if (GetLastTargetGroup(groups, note, entityType) is NoteItemGroup targetGroup)
        {
            targetGroup.InsertSorted(note, NoteItemComparer.Instance);
        }
        else
        {
            targetGroup = new NoteItemGroup([note], entityType, notePageNumberHeaderTemplate);

            var comparer =
                entityType == EntityType.Case
                    ? NoteItemGroupComparer.NotePeriodInstance
                    : NoteItemGroupComparer.PageNumberInstance;

            groups.InsertSorted(targetGroup, comparer);
        }
    }

    private static (int, int) GetJaggedIndex(ObservableCollection<NoteItemGroup> groups, int flattenedIndex)
    {
        int matchIndex = 0;

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            var group = groups[groupIndex];

            if (matchIndex + group.Count <= flattenedIndex)
                matchIndex += group.Count;
            else
                return (groupIndex, flattenedIndex - matchIndex);
        }

        return (-1, -1);
    }

    public static void RemoveFromSortedGroups(ObservableCollection<NoteItemGroup> groups, int flattenedIndex)
    {
        if (groups == null || !groups.Any())
            return;

        var (groupIndex, noteIndex) = GetJaggedIndex(groups, flattenedIndex);

        if (groupIndex == -1 || noteIndex == -1)
            return;

        var targetGroup = groups[groupIndex];

        targetGroup.RemoveAt(noteIndex);

        if (!targetGroup.Any())
            groups.Remove(targetGroup);
    }
}
