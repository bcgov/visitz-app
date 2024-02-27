using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Visitz.Behaviors;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Notes;
using VisitzModel.Models;

namespace Visitz.ViewModels.Entity;

public partial class EntityNotesViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public ObservableCollection<NoteItemGroup> notes;

    [ObservableProperty]
    public bool isNotesEmtpy;

    private IQueryable<NoteItem> NoteItemsQuery { get; set; }

    private IDisposable NoteItemsQueryToken { get; set; }

    public NoteItemGroup LastNoteItemGroup => Notes?.LastOrDefault();

    public NoteItem LastNoteItem => LastNoteItemGroup?.LastOrDefault();

    public override async void Create()
    {
        base.Create();

        var realm = await VisitzRealms.GetIcmDataRealmAsync();

        NoteItemsQuery = NoteItem.GetNotesByEntityId(realm, CaseloadItem.CaseIncidentNumber);
        NoteItemsQueryToken = NoteItemsQuery.SubscribeForNotifications(NoteItemsQuery_Changed);
    }

    public override void Destroy()
    {
        if (Notes != null)
            Notes.CollectionChanged -= Notes_CollectionChanged;
        Notes = null;

        NoteItemsQueryToken?.Dispose();
        NoteItemsQueryToken = null;

        base.Destroy();
    }

    private void InitNotesCollection(List<NoteItemGroup> items)
    {
        if (Notes != null)
            Notes.CollectionChanged -= Notes_CollectionChanged;

        Notes = new ObservableCollection<NoteItemGroup>(items);
        Notes.CollectionChanged += Notes_CollectionChanged;
        
        IsNotesEmtpy = items.Count == 0;
    }

    private void Notes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        IsNotesEmtpy = !Notes?.Any() ?? true;
    }

    private void NoteItemsQuery_Changed(IRealmCollection<NoteItem> realmNotes, ChangeSet changes)
    {
        if (changes == null)
        {
            var groups = NoteItemGroup.GetGroupsFromNotesQuery(CaseloadItem.EntityType, NoteItemsQuery,
                LocalizedStrings.NotePageNumberHeader);
            InitNotesCollection(groups);
            return;
        }

        if (changes.IsCleared)
        {
            Notes.Clear();
            return;
        }

        foreach (var deletedIndex in changes.DeletedIndices.Reverse())
            NoteItemGroup.RemoveFromSortedGroups(Notes, deletedIndex);

        foreach (var insertedIndex in changes.InsertedIndices)
            NoteItemGroup.InsertInSortedGroups(Notes, realmNotes[insertedIndex], CaseloadItem.EntityType,
                LocalizedStrings.NotePageNumberHeader);
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
		noteEntryPage.Behaviors.Add(new SoftPageKeyboardBehavior());

        await Navigator.Navigation.PushModalAsync(noteEntryPage);
    }
}
