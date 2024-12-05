namespace Visitz.Views.BaseClasses;

public abstract class ViewModelContentView(VisitzViewModel viewModel) : BaseContentView
{
	protected VisitzViewModel ViewModel { get; set; } = viewModel;

	protected override void Creating()
	{
		ViewModel.OnCreate();
	}

	protected override void Destroying()
	{
		ViewModel.Destroy();
	}
}
