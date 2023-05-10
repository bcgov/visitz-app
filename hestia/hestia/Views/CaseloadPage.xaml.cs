using hestia.ViewModels;
using hestia.Routers;
using hestia.Models.BOs;

namespace hestia.Views;

public partial class CaseloadPage : BasePage
{
    private CaseloadViewModel viewModel;
    private CaseloadRouter router;

    public CaseloadPage(CaseloadViewModel viewModel, CaseloadRouter router)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.router = router;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        viewModel.FetchCasesAndIncidents();
        ListenToViewModelProperties();
    }

    private void ListenToViewModelProperties()
    {
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName.Equals(nameof(viewModel.SelectedCaseIncident)) &&
                viewModel.SelectedCaseIncident is not null)
            {
                TriggerRouteUpdate(viewModel.SelectedCaseIncident);
            }
        };
    }

    private void TriggerRouteUpdate(ListCaseIncident2 caseIncident)
    {
        router.RouteUsing(caseIncident);
    }
}

