using Visitz.ViewModels;

namespace Visitz.Views;

public partial class DebugOptionsPage : VisitzPage
{
	public DebugOptionsPage(DebugOptionsViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}