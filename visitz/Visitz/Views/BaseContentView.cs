using Visitz.Extensions;

namespace Visitz.Views;

public abstract class BaseContentView : ContentView
{
    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        if (args.AttachingToParent())
            Creating();
        else if (args.DetachingFromParent())
            Destroying();
    }

    protected virtual void Creating() { }

    protected virtual void Destroying() { }
}