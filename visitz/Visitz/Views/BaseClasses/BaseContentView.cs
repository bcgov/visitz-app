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
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Creating(); // Used until all other references are removed
#pragma warning restore CS0618 // Type or member is obsolete

            InitTask = InitAsync();
        }
        else if (args.DetachingFromHandler())
#pragma warning disable CS0618 // Type or member is obsolete
            Destroying(); // Used until all other references are removed
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Obsolete("Use InitAsync instead")]
    protected virtual void Creating()
    {
        ConsoleTrace.TraceMethod(this);
    }

    protected virtual Task InitAsync()
    {
        ConsoleTrace.TraceMethod(this);

        return Task.CompletedTask;
    }

    [Obsolete("Use Dispose instead")]
    protected virtual void Destroying()
    {
        ConsoleTrace.TraceMethod(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
        {
            ConsoleTrace.TraceMethod(this);
            Handler = null;
        }

        _disposedValue = true;
    }
}
