using Visitz.ViewModels;
using Visitz.Routers;
using Visitz.Models.BOs;

namespace Visitz.Views;

public partial class CaseloadPage : VisitzPage
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

    private void TriggerRouteUpdate(CaseloadItem caseIncident)
    {
        router.RouteUsing(caseIncident);
    }
}
