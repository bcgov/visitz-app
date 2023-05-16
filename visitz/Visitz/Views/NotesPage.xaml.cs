using Visitz.ViewModels;

namespace Visitz.Views;

public partial class NotesPage : VisitzPage
{
    public NotesPage(NotesViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    async void CaseDetailsTapped(object sender, EventArgs e)
    {
        if (ViewModel is NotesViewModel notesVm)
            await notesVm.CaseDetailsTapped();
    }
}
