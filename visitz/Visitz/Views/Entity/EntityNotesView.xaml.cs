using Visitz.Models;

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

    private void NotesCollectionView_Loaded(object sender, EventArgs e)
    {
		var entityNotesVM = ViewModel as EntityNotesViewModel;

        var last = entityNotesVM.LastNoteItem;
		var lastGroup = entityNotesVM.LastNoteItemGroup;

		ScrollToItem(last, lastGroup);
    }

	private void ScrollToItem(NoteItem item, NoteItemGroup noteItemGroup)
	{
		if (item != null && noteItemGroup != null)
        	NotesCollectionView.ScrollTo(item, noteItemGroup, position: ScrollToPosition.End, animate: false);
	}
}