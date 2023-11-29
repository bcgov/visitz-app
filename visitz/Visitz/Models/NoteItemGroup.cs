using Realms;
using System.Collections.ObjectModel;
using System.Globalization;
using Visitz.Extensions;
using Visitz.Models.Comparers;
using Visitz.Resources.Localization;

namespace Visitz.Models;

public class NoteItemGroup : ObservableCollection<NoteItem>
{
    private static readonly string DistinctQuery = "TRUEPREDICATE DISTINCT({0})";

    public string Name => EntityType == IcmEntity.Case
        ? NotePeriodDateTime.ToString(NoteItem.IcmNotePeriodDateFormat, CultureInfo.InvariantCulture)
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
}
