using Visitz.ViewModels.Entity;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

public partial class EntityNotesView : ViewModelContentView, ICaseloadItemHolder, ISelectedEntitySection
{
	new EntityNotesViewModel ViewModel => base.ViewModel as EntityNotesViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
    }

	public EntitySection SelectedSection
	{
		get => ViewModel.SelectedSection;
		set => ViewModel.SelectedSection = value;
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
#if IOS
		await Task.Run(() => SpinWait.SpinUntil(() =>
			ViewModel.LastNoteItem != null && ViewModel.LastNoteItemGroup != null));
#endif
        ScrollToItem(ViewModel.LastNoteItem, ViewModel.LastNoteItemGroup);
    }

	private void ScrollToItem(NoteItem item, NoteItemGroup noteItemGroup)
	{
		if (item != null && noteItemGroup != null)
        	NotesCollectionView.ScrollTo(item, noteItemGroup, position: ScrollToPosition.End, animate: false);
	}
}
