using Visitz.Extensions;
using VisitzModel;

namespace Visitz.Views.BaseClasses;

public abstract partial class VisitzPage(VisitzViewModel visitzViewModel) : ContentPage() 
{
    protected VisitzViewModel ViewModel { get; set; } = visitzViewModel;

    protected Window CurrentWindow => Window ?? GetParentWindow();

    protected virtual void OnCreated() 
    {
        ConsoleTrace.TraceMethod(this);

        ViewModel.OnCreate();
    }

    protected virtual void OnDestroyed()
    {
        ConsoleTrace.TraceMethod(this);

        Behaviors.Clear();
        ViewModel.Destroy();
    }

    protected override bool OnBackButtonPressed()
    {
        ConsoleTrace.TraceMethod(this);
        return base.OnBackButtonPressed();
    }

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        var isCreating = args.AttachingToParent();
        var isDestroying = args.DetachingFromParent();

        if (isCreating)
            OnCreated();
        else if (isDestroying)
            OnDestroyed();
    }
}
