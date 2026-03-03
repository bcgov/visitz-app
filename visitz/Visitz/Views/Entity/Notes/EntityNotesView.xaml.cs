using System.Text;
using Visitz.Views.BaseClasses;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;

namespace Visitz.Views.Entity.Notes;

public partial class EntityNotesView :
    ViewModelContentView,
    IBusinessObjectHolder,
    IRequestedEntitySection
{
    new EntityNotesViewModel ViewModel => base.ViewModel as EntityNotesViewModel;

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
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

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        if (BindingContext is EntityNotesViewModel viewModel)
        {
            if (viewModel.Notes == null || !viewModel.Notes.Any())
                return;
            var builder = new StringBuilder();
            foreach (var group in viewModel.Notes)
            {
                // Add Group Name
                builder.AppendLine(group.Name);
                builder.AppendLine(new string('-', group.Name.Length));
                foreach (var item in group)
                {
                    builder.AppendLine(item.Content);
                }
                builder.AppendLine(); // spacing between groups
            }
            string finalText = builder.ToString();
            await Clipboard.Default.SetTextAsync(finalText);
        }
    }
}
