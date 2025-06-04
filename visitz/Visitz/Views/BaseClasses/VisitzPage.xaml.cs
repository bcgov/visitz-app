using Visitz.Extensions;
using VisitzModel;

namespace Visitz.Views.BaseClasses;

public partial class VisitzPage(VisitzViewModel visitzViewModel) : ContentPage(), IDisposable
{
    private bool _disposed;

    protected VisitzViewModel ViewModel { get; set; } = visitzViewModel;

    protected Task ViewModelInit { get; private set; }

    protected Window CurrentWindow => Window ?? GetParentWindow();

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        var isCreating = args.AttachingToParent();
        var isDestroying = args.DetachingFromParent();

        if (isCreating)
            OnCreated();
        else if (isDestroying)
            Dispose();
    }

    protected virtual void OnCreated()
    {
        ConsoleTrace.TraceMethod(this);

        ViewModelInit = ViewModel?.StartInitAsync();
    }

    protected virtual void OnDestroyed()
    {
        ConsoleTrace.TraceMethod(this);

        Behaviors.Clear();

        ViewModel?.Dispose();

        if (Content.FindFirstDisposable() is IDisposable disposable)
            disposable.Dispose();
    }

    protected override bool OnBackButtonPressed()
    {
        ConsoleTrace.TraceMethod(this);
        return base.OnBackButtonPressed();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                OnDestroyed();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
