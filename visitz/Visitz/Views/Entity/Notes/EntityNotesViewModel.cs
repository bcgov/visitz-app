using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class EntityNotesViewModel :
    VisitzViewModel,
    IBusinessObjectHolder,
    IRequestedEntitySection
{
    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public ObservableCollection<NoteItemGroup> notes;

    [ObservableProperty]
    public bool isNotesEmtpy;

    private readonly ObservableRealmQueryMap realmQueryMap = new();

    public NoteItemGroup LastNoteItemGroup => Notes?.LastOrDefault();

    public NoteItem LastNoteItem => LastNoteItemGroup?.LastOrDefault();

    [ObservableProperty]
    public EntitySection requestedSection;

    [ObservableProperty]
    public string openNoteEntryText;

    public readonly TaskCompletionSource notesLoadedTcs = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        var realm = await VisitzRealms.GetIcmDataRealmAsync();

        realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;
        realmQueryMap.Subscribe(realm, NoteItem.GetNotesByFileNumber(realm, BusinessObject.FileNumber));

        var noteDraftRealm = await VisitzRealms.GetNoteDraftsRealmAsync();

        realmQueryMap.Subscribe(noteDraftRealm, noteDraftRealm.All<NoteDraft>()
            .Where(draft => draft.ParentEntityId == BusinessObject.FileNumber));

        if (RequestedSection == EntitySection.NoteEntry)
            await OpenNoteEntry();
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            if (Notes != null)
                Notes.CollectionChanged -= Notes_CollectionChanged;
            Notes = null;

            realmQueryMap.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
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

    private void RealmQueryMap_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(NoteItem))
            UpdateNotesList(e.Items as IRealmCollection<NoteItem>, e.Changes);
        else if (e.Type == typeof(NoteDraft))
            UpdateOpenNoteEntryText(e.Items.Any());
    }

    private void UpdateNotesList(IRealmCollection<NoteItem> realmNotes, ChangeSet changes)
    {
        if (changes == null)
        {
            var groups = NoteItemGroup.GetGroupsFromNotesQuery(
                BusinessObject.EntityType,
                realmQueryMap[typeof(NoteItem)].Query as IQueryable<NoteItem>,
                LocalizedStrings.NotePageNumberHeader
            );

            InitNotesCollection(groups);

            notesLoadedTcs.TrySetResult();
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
            NoteItemGroup.InsertInSortedGroups(Notes, realmNotes[insertedIndex],
                BusinessObject.EntityType, LocalizedStrings.NotePageNumberHeader);
    }

    private void UpdateOpenNoteEntryText(bool draftAvailable)
    {
        OpenNoteEntryText = draftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddNotes;
    }

    [RelayCommand]
    public async Task AddNote()
    {
        await OpenNoteEntry();
    }

    private async Task OpenNoteEntry()
    {
        var noteEntryView = ServiceProvider.GetService<NoteEntryView>();
        noteEntryView.BusinessObject = BusinessObject;

        await Navigator.Navigation.PushModalAsync(noteEntryView, ViewModalSize.Wide);
    }
}
