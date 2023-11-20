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
		var entityNotesVM = (ViewModel as EntityNotesViewModel);

        var last = entityNotesVM.LastNoteItem;
		var lastGroup = entityNotesVM.LastNoteItemGroup;

		if (last != null && lastGroup != null)
        	NotesCollectionView.ScrollTo(last, lastGroup, position: ScrollToPosition.End, animate: false);
    }
}