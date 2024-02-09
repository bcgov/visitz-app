using Visitz.ViewModels;

namespace Visitz.Views;

public abstract class ViewModelContentView(VisitzViewModel viewModel) : BaseContentView
{
    protected VisitzViewModel ViewModel { get; set; } = viewModel;

	protected override void Creating()
	{
		ViewModel.Create();
	}

    protected override void Destroying()
	{
        ViewModel.Destroy();
    }
}