using hestia.ViewModels;
using hestia.Routers;

namespace hestia.Views;

public partial class NotesPage : hestia.Views.BasePage
{
	private NotesViewModel viewModel;
	private NotesRouter router;

    public NotesPage(NotesViewModel viewModel, NotesRouter router)
	{
		InitializeComponent();
		BindingContext = viewModel;
		this.viewModel = viewModel;
        this.router = router;
    }

    protected override void OnLoad() {
		base.OnLoad();
		viewModel.FetchNotes();
	}

    void CaseDetailsTapped(System.Object sender, System.EventArgs e)
    {
		router.RouteUsing(viewModel.CaseIncident);
    }
}
