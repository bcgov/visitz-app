namespace Visitz.Views.BaseClasses;

public abstract class ViewModelContentView(VisitzViewModel viewModel) : BaseContentView
{
    private bool _disposedValue;

	protected VisitzViewModel ViewModel { get; set; } = viewModel;

    [Obsolete("Use InitAsync instead")]
	protected override void Creating()
	{
		ViewModel.OnCreate();
	}

    [Obsolete("Use Dispose instead")]
	protected override void Destroying()
	{
		ViewModel.Dispose();
	}

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _ = ViewModel.StartInitAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
#pragma warning disable CS0618 // Type or member is obsolete
                Destroying(); // Used until all other references are removed
#pragma warning restore CS0618 // Type or member is obsolete

            _disposedValue = true;
        }

        base.Dispose(disposing);
    }
}
