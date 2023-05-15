using Visitz.ViewModels;
using Visitz.Routers;

namespace Visitz.Views;

public partial class NotesPage : Visitz.Views.VisitzPage
{
	private NotesViewModel viewModel;
	private NotesRouter router;

    public NotesPage(NotesViewModel viewModel, NotesRouter router) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		this.viewModel = viewModel;
        this.router = router;
    }

    protected override void OnLoad()
	{
		viewModel.FetchNotes();
	}

    void CaseDetailsTapped(System.Object sender, System.EventArgs e)
    {
		router.RouteUsing(viewModel.CaseIncident);
    }
}
