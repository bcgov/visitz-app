using Visitz.Extensions;
using VisitzModel;

namespace Visitz.Views.BaseClasses;

public abstract class BaseContentView : ContentView, IDisposable
{
    private bool _disposedValue;

    public Task InitTask { get; private set; }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.AttachingToHandler())
            InitTask = InitAsync();
        else if (args.DetachingFromHandler())
            Dispose();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual Task InitAsync()
    {
        ConsoleTrace.TraceMethod(this);

        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            ConsoleTrace.TraceMethod(this);

        _disposedValue = true;
    }
}
