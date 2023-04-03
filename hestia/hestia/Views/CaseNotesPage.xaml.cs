using hestia.ViewModels;
using hestia.Routers;

namespace hestia.Views;

public partial class CaseNotesPage : hestia.Views.BasePage
{
	private CaseNotesViewModel viewModel;
	private CaseNotesRouter router;

    public CaseNotesPage(CaseNotesViewModel viewModel, CaseNotesRouter router)
	{
		InitializeComponent();
		BindingContext = viewModel;
		this.viewModel = viewModel;
        this.router = router;
    }

    protected override void OnLoad() {
		base.OnLoad();
		viewModel.FetchCaseNotes();
	}

    void CaseDetailsTapped(System.Object sender, System.EventArgs e)
    {
		router.RouteUsing(viewModel.CaseIncident);
    }
}
