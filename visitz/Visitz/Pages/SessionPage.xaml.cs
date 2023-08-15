using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class SessionPage : VisitzPage
{
	public SessionPage(SessionViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}