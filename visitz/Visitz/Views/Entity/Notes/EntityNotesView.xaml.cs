using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class EntityNotesView : ViewModelContentView, ICaseloadItemHolder, IRequestedEntitySection
{
	new EntityNotesViewModel ViewModel => base.ViewModel as EntityNotesViewModel;

	public CaseloadItem CaseloadItem
	{
		get => ViewModel.CaseloadItem;
		set => ViewModel.CaseloadItem = value;
    }

	public EntitySection RequestedSection
	{
		get => ViewModel.RequestedSection;
		set => ViewModel.RequestedSection = value;
	}

	public EntityNotesView() : base(ServiceProvider.GetService<EntityNotesViewModel>())
	{
		InitializeComponent();
		BindingContext = ViewModel;
	}

    private async void NotesCollectionView_Loaded(object sender, EventArgs e)
    {
		await ViewModel.notesLoadedTcs.Task;

        ScrollToItem(ViewModel.LastNoteItem, ViewModel.LastNoteItemGroup);
    }

	private void ScrollToItem(NoteItem item, NoteItemGroup noteItemGroup)
	{
		if (item != null && noteItemGroup != null)
        	NotesCollectionView.ScrollTo(item, noteItemGroup, position: ScrollToPosition.End, animate: false);
	}
}
