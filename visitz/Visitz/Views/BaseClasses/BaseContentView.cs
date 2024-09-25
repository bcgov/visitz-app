using Visitz.Extensions;
using VisitzModel;

namespace Visitz.Views.BaseClasses;

public abstract class BaseContentView : ContentView
{
	protected override void OnHandlerChanging(HandlerChangingEventArgs args)
	{
		base.OnHandlerChanging(args);

		if (args.AttachingToHandler())
			Creating();
		else if (args.DetachingFromHandler())
			Destroying();
	}

	public void Destroy()
	{
		Handler = null;
	}

	protected virtual void Creating()
    {
        ConsoleTrace.TraceMethod(this);
    }

	protected virtual void Destroying()
    {
        ConsoleTrace.TraceMethod(this);
    }
}
