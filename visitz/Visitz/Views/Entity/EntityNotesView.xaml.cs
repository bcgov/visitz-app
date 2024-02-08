using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntityNotesView : ViewModelContentView, ICaseloadItemHolder
{
	public CaseloadItem CaseloadItem
	{
		get => (ViewModel as ICaseloadItemHolder).CaseloadItem;
        set => (ViewModel as ICaseloadItemHolder).CaseloadItem = value;
    }

	public EntityNotesView() : base(ServiceProvider.GetService<EntityNotesViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

	// Because lifecycle events are processed in a different order between iOS & Windows, this Loaded event has
	// specific logic for either platform.
	// TODO: Clean this up and use proper async loading with progress indicators in the UI—and/or improve general
	// performance.
#if IOS
    private async void NotesCollectionView_Loaded(object sender, EventArgs e)
#else
    private void NotesCollectionView_Loaded(object sender, EventArgs e)
#endif
    {
        var entityNotesVM = ViewModel as EntityNotesViewModel;
#if IOS
		await Task.Run(() => SpinWait.SpinUntil(() => 
			entityNotesVM.LastNoteItem != null && entityNotesVM.LastNoteItemGroup != null));
#endif
        ScrollToItem(entityNotesVM.LastNoteItem, entityNotesVM.LastNoteItemGroup);
    }

	private void ScrollToItem(NoteItem item, NoteItemGroup noteItemGroup)
	{
		if (item != null && noteItemGroup != null)
        	NotesCollectionView.ScrollTo(item, noteItemGroup, position: ScrollToPosition.End, animate: false);
	}
}