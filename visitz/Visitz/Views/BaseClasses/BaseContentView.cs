using Visitz.Extensions;
using VisitzModel;

namespace Visitz.Views.BaseClasses;

public abstract class BaseContentView : ContentView, IDisposable
{
    private bool _disposedValue;

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
	{
		base.OnHandlerChanging(args);

		if (args.AttachingToHandler())
			Creating();
		else if (args.DetachingFromHandler())
			Destroying();
	}

	public void Dispose()
	{
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

	protected virtual void Creating()
    {
        ConsoleTrace.TraceMethod(this);
    }

	protected virtual void Destroying()
    {
        ConsoleTrace.TraceMethod(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            Handler = null;

        _disposedValue = true;
    }
}
