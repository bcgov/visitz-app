using Visitz.ViewModels;
namespace Visitz.Views;

public partial class CaseloadItemDetailsPage : ContentPage
{
	private CaseloadItemDetailsViewModel viewModel;
    public CaseloadItemDetailsPage(CaseloadItemDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        this.viewModel = viewModel;
    }
}
