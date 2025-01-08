namespace Visitz.Views.BaseClasses;

public abstract class ViewModelContentView(VisitzViewModel viewModel) : BaseContentView
{
    private bool _disposedValue;

	protected VisitzViewModel ViewModel { get; set; } = viewModel;

	protected override void Creating()
	{
		ViewModel.OnCreate();
	}

	protected override void Destroying()
	{
		ViewModel.Dispose();
	}

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                Destroying();

            _disposedValue = true;
        }

        base.Dispose(disposing);
    }
}
