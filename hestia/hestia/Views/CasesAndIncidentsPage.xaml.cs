using hestia.ViewModels;
namespace hestia.Views;

public partial class CasesAndIncidentsPage : BasePage
{
    private CasesAndIncidentsViewModel viewModel;

    public CasesAndIncidentsPage(CasesAndIncidentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        viewModel.FetchCasesAndIncidents();
    }
}
