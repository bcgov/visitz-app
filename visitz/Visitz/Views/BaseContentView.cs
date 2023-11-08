using Visitz.Extensions;

namespace Visitz.Views;

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

    protected virtual void Creating() { }

    protected virtual void Destroying() { }
}