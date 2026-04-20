namespace Visitz.Views.BaseClasses;

#nullable enable

public abstract class ViewModelContentView(VisitzViewModel viewModel, string title = "") : BaseContentView(title)
{
    public VisitzViewModel ViewModel { get; private set; } = viewModel;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        ArgumentNullException.ThrowIfNull(ViewModel);

        await ViewModel.StartInitAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            ViewModel.Dispose();

        base.Dispose(disposing);
    }
}
