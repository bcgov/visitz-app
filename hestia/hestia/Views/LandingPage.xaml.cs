using hestia.ViewModels;
using hestia.Routers;

namespace hestia.Views;

public partial class LandingPage : BasePage
{
    LandingRouter router;
    LandingViewModel viewModel;

    public LandingPage(LandingViewModel viewModel, LandingRouter router)
	{
		InitializeComponent();
        BindingContext = viewModel;
        this.viewModel = viewModel;
        this.router = router;
        ListenToViewModelProperties();
	}

    protected override void OnLoadAsync()
    {
        base.OnLoadAsync();
        //TriggerRouteUpdate();

        LandingRouter.Route solvedRoute = viewModel.SolveRoute();
        router.RouteTo(solvedRoute);
    }

    void ListenToViewModelProperties()
    {
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName.Equals(nameof(viewModel.BackFromPage)))
            {
                //Shell.Current.CurrentState

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
