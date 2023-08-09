using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class UserSessionPage : VisitzPage
{
	public UserSessionPage(UserSessionViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}