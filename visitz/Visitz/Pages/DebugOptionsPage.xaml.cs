using Visitz.ViewModels;

namespace Visitz.Pages;

public partial class DebugOptionsPage : VisitzPage
{
	public DebugOptionsPage(DebugOptionsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}