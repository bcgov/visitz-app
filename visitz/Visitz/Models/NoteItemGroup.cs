using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;

namespace Visitz.Models;

public class NoteItemGroup(string name, List<NoteItem> items) : List<NoteItem>(items)
{
    private static readonly string DistinctQuery = "TRUEPREDICATE DISTINCT({0})";

    public string Name { get; set; } = name;

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

        return new NoteItemGroup(notePeriod, notesForPeriod);
    }

    private static NoteItemGroup GetNotesGroupByPage(int pageNumber, IQueryable<NoteItem> entityNotesQuery)
    {
        var notesForPage = entityNotesQuery
            .Where(item => item.PageNumber == pageNumber)
            .AsEnumerable()
            .OrderBy(item => DateTime.Parse(item.CreatedDate))
            .ToList();

        string groupName = LocalizedStrings.NotePageNumberHeader.Format(pageNumber);
        return new NoteItemGroup(groupName, notesForPage);
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
}
