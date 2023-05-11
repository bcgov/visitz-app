using visitz.ViewModels;
namespace visitz.Views;

public partial class CaseIncidentDetailsPage : ContentPage
{
	private CaseIncidentDetailsViewModel viewModel;
    public CaseIncidentDetailsPage(CaseIncidentDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        this.viewModel = viewModel;
    }
}
