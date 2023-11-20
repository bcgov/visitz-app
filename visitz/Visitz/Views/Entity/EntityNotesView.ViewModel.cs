using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Models;
using Visitz.Storage;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntityNotesViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public List<NoteItemGroup> notes;

    private Realm Realm { get; set; }

    private IQueryable<NoteItem> NoteItemsQuery { get; set; }

    private IDisposable NoteItemsQueryToken { get; set; }

    public NoteItemGroup LastNoteItemGroup => Notes.LastOrDefault();

    public NoteItem LastNoteItem => LastNoteItemGroup?.LastOrDefault();

    public override async void PageCreated()
    {
        base.PageCreated();

        Realm = await VisitzRealm.GetIcmDataAsync();

        NoteItemsQuery = Realm
            .All<NoteItem>()
            .Where(item => item.IcmId == CaseloadItem.CaseIncidentNumber);

        NoteItemsQueryToken = NoteItemsQuery.SubscribeForNotifications(NoteItems_Changed);

        ApplyNoteItemsQuery();
    }

    public override void PageDestroyed()
    {
        NoteItemsQueryToken?.Dispose();
        NoteItemsQueryToken = null;

        Realm?.Dispose();
        Realm = null;

        base.PageDestroyed();
    }

    private void ApplyNoteItemsQuery()
    {
        Notes = NoteItemGroup.GetGroupsFromNotesQuery(CaseloadItem.EntityType, NoteItemsQuery);
    }

    private void NoteItems_Changed(IRealmCollection<NoteItem> noteItems, ChangeSet changes)
    {
        if (changes == null)
            return;

        ApplyNoteItemsQuery();
    }

    [RelayCommand]
    public void AddNote()
    {
        // TODO: Open NoteEntryView
    }
}
