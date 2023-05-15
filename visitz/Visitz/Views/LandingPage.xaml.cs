using Visitz.ViewModels;
using Visitz.Routers;

namespace Visitz.Views;

public partial class LandingPage : VisitzPage
{
    LandingRouter router;
    LandingViewModel viewModel;

    public LandingPage(LandingViewModel viewModel, LandingRouter router) : base(viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.router = router;
        ListenToViewModelProperties();
    }

    protected override void OnLoad()
    {
        LandingRouter.Route solvedRoute = viewModel.SolveRoute();
        router.RouteTo(solvedRoute);
    }

    void ListenToViewModelProperties()
    {
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName.Equals(nameof(viewModel.BackFromPage)))
            {
                TriggerRouteUpdate();
            }
        };
    }

    void TriggerRouteUpdate()
    {
        LandingRouter.Route solvedRoute = viewModel.SolveRoute();
        router.RouteTo(solvedRoute);
    }
}
