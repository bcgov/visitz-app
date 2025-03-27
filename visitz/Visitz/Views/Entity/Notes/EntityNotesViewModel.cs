using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class EntityNotesViewModel : VisitzViewModel, ICaseloadItemHolder, IRequestedEntitySection
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

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

	public override async void Create()
    {
        base.Create();

        var realm = await VisitzRealms.GetIcmDataRealmAsync();

		realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;
		realmQueryMap.Subscribe(realm, NoteItem.GetNotesByEntityId(realm, CaseloadItem.CaseIncidentNumber));

		var noteDraftRealm = await VisitzRealms.GetNoteDraftsRealmAsync();

		realmQueryMap.Subscribe(noteDraftRealm, noteDraftRealm.All<NoteDraft>()
			.Where(draft => draft.ParentEntityId == CaseloadItem.CaseIncidentNumber));

		if (RequestedSection == EntitySection.NoteEntry)
			await OpenNoteEntry();
    }

	public override void Destroy()
    {
        if (Notes != null)
            Notes.CollectionChanged -= Notes_CollectionChanged;
        Notes = null;

		realmQueryMap.Dispose();

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
				CaseloadItem.EntityType.ParseEntityType(),
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
				CaseloadItem.EntityType.ParseEntityType(), LocalizedStrings.NotePageNumberHeader);
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
        noteEntryView.CaseloadItem = CaseloadItem;

        await Navigator.Navigation.PushModalAsync(noteEntryView, ViewModalSize.Wide);
    }
}
