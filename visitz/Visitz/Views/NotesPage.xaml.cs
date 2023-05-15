using Visitz.ViewModels;
using Visitz.Routers;

namespace Visitz.Views;

public partial class NotesPage : VisitzPage
{
    public NotesPage(NotesViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    void CaseDetailsTapped(object sender, EventArgs e)
    {
        if (ViewModel is NotesViewModel notesVm)
            notesVm.CaseDetailsTapped();
    }
}
