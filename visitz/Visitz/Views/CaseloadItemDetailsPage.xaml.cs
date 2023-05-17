using Visitz.ViewModels;
namespace Visitz.Views;

public partial class CaseloadItemDetailsPage : VisitzPage
{
    public CaseloadItemDetailsPage(CaseloadItemDetailsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}
