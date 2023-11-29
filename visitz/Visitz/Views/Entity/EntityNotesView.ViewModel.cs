using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Visitz.Extensions;
using Visitz.Models;
using Visitz.Storage;
using Visitz.ViewModels;
using Visitz.Views.Notes;

namespace Visitz.Views.Entity;

public partial class EntityNotesViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public ObservableCollection<NoteItemGroup> notes = new();

    [ObservableProperty]
    public bool isNotesEmtpy;

    private Realm Realm { get; set; }

    private IQueryable<NoteItem> NoteItemsQuery { get; set; }

    private IDisposable NoteItemsQueryToken { get; set; }

    public NoteItemGroup LastNoteItemGroup => Notes?.LastOrDefault();

    public NoteItem LastNoteItem => LastNoteItemGroup?.LastOrDefault();

    public override async void PageCreated()
    {
        base.PageCreated();

        Realm = await VisitzRealm.GetIcmDataAsync();

        Notes.CollectionChanged += Notes_CollectionChanged;

        NoteItemsQuery = NoteItem.GetNotesByEntityId(Realm, CaseloadItem.CaseIncidentNumber);
        NoteItemsQuery.AsRealmCollection().CollectionChanged += NoteItemsQuery_CollectionChanged;

        var groups = NoteItemGroup.GetGroupsFromNotesQuery(CaseloadItem.EntityType, NoteItemsQuery);
        IsNotesEmtpy = groups.Count == 0;

        foreach (var note in groups)
            Notes.Add(note);
    }

    public override void PageDestroyed()
    {
        Notes = null;

        NoteItemsQueryToken?.Dispose();
        NoteItemsQueryToken = null;

        NoteItemsQuery.AsRealmCollection().CollectionChanged -= NoteItemsQuery_CollectionChanged;
        Notes.CollectionChanged -= Notes_CollectionChanged;

        Realm?.Dispose();
        Realm = null;

        base.PageDestroyed();
    }

    private void Notes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        IsNotesEmtpy = !Notes?.Any() ?? true;
    }

    private void NoteItemsQuery_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                NoteItem addNote = (NoteItem)e.NewItems[0];
                NoteItemGroup.InsertInSortedGroups(Notes, addNote, CaseloadItem.EntityType);
                break;
            case NotifyCollectionChangedAction.Remove:
                NoteItemGroup.RemoveFromSortedGroups(Notes, e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                Notes.Clear();
                break;

            default:
                throw new NotImplementedException();
        }
    }

    [RelayCommand]
    public async void AddNote()
    {
        await OpenNoteEntry();
    }

    private async Task OpenNoteEntry()
    {
        var noteEntryView = ServiceProvider.GetService<NoteEntryView>();
        noteEntryView.CaseloadItem = CaseloadItem;

        var noteEntryPage = noteEntryView.WrapPageForModal();
        await Navigator.Navigation.PushModalAsync(noteEntryPage);
    }
}
