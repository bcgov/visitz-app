using Realms;
using System.Collections.ObjectModel;
using Visitz.Models.Comparers;
using Visitz.Resources.Localization;
using VisitzModel.Extensions;

namespace Visitz.Models;

public class NoteItemGroup : ObservableCollection<NoteItem>
{
    private static readonly string DistinctQuery = "TRUEPREDICATE DISTINCT({0})";

    public string Name => EntityType == IcmEntity.Case
        ? NoteItem.NotePeriodFrom(NotePeriodDateTime)
        : MakePageNumberHeader(PageNumber);

    public DateTimeOffset NotePeriodDateTime { get; private set; }

    public int PageNumber { get; private set; }

    public string EntityType { get; private set; }

    public NoteItemGroup(NoteItem note, string entityType) : base()
    {
        NotePeriodDateTime = note.NotePeriodDateTime;
        PageNumber = note.PageNumber;
        EntityType = entityType;
        Add(note);
    }

    public NoteItemGroup(List<NoteItem> notes, string entityType) : base(notes)
    {
        var note = notes.First();

        NotePeriodDateTime = note.NotePeriodDateTime;
        PageNumber = note.PageNumber;
        EntityType = entityType;
    }

    private static string MakePageNumberHeader(int pageNumber)
    {
        return LocalizedStrings.NotePageNumberHeader.Format(pageNumber);
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
                .OrderBy(item => DateTime.Parse(item.CreatedDate))
                .ToList();

        return new NoteItemGroup(notesForPeriod, IcmEntity.Case);
    }

    private static NoteItemGroup GetNotesGroupByPage(int pageNumber, IQueryable<NoteItem> entityNotesQuery)
    {
        var notesForPage = entityNotesQuery
            .Where(item => item.PageNumber == pageNumber)
            .AsEnumerable()
            .OrderBy(item => DateTime.Parse(item.CreatedDate))
            .ToList();

        string groupName = MakePageNumberHeader(pageNumber);
        return new NoteItemGroup(notesForPage, IcmEntity.Incident);
    }

    public static List<NoteItemGroup> GetGroupsFromNotesQuery(string icmEntityType, IQueryable<NoteItem> entityNotesQuery)
    {
        var groups = new List<NoteItemGroup>();

        if (icmEntityType == IcmEntity.Case)
        {
            foreach (var notePeriod in GetPeriodHeaders(entityNotesQuery))
                groups.Add(GetNotesGroupByPeriod(notePeriod, entityNotesQuery));
        }
        else
        {
            foreach (var pageNumber in GetPageHeaders(entityNotesQuery))
                groups.Add(GetNotesGroupByPage(pageNumber, entityNotesQuery));
        }

        return groups;
    }

    private static NoteItemGroup GetLastTargetGroup(IList<NoteItemGroup> groups, NoteItem note, string entityType)
    {
        return entityType == IcmEntity.Case
            ? groups.LastOrDefault(group => group.Name == note.NotePeriod)
            : groups.LastOrDefault(group => group.Name == MakePageNumberHeader(note.PageNumber));
    }

    public static void InsertInSortedGroups(ObservableCollection<NoteItemGroup> groups, NoteItem note, string entityType)
    {
        var targetGroup = GetLastTargetGroup(groups, note, entityType);

        if (targetGroup == null)
        {
            targetGroup = new NoteItemGroup(note, entityType);

            var comparer = entityType == IcmEntity.Case 
                ? NoteItemGroupComparer.NotePeriodInstance 
                : NoteItemGroupComparer.PageNumberInstance;

            int groupIndex = groups.BinarySearch(targetGroup, comparer);
            if (groupIndex < 0)
                groupIndex = ~groupIndex;

            groups.Insert(groupIndex, targetGroup);
        }
        else
        {
            var notes = (ObservableCollection<NoteItem>)targetGroup;

            int noteIndex = notes.BinarySearch(note, NoteItemComparer.Instance);
            if (noteIndex < 0)
                noteIndex = ~noteIndex;

            notes.Insert(noteIndex, note);
        }
    }

    private static (int,int) GetJaggedIndex(ObservableCollection<NoteItemGroup> groups, int flattenedIndex)
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
