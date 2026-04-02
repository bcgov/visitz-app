namespace Visitz.Views.BaseClasses;

#nullable enable

public abstract class ViewModelContentView(VisitzViewModel viewModel) : BaseContentView
{
    protected VisitzViewModel ViewModel { get; set; } = viewModel;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _ = ViewModel.StartInitAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            ViewModel.Dispose();

        base.Dispose(disposing);
    }
}
