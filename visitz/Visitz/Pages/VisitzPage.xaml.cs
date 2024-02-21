using Visitz.Extensions;
using Visitz.ViewModels;
using VisitzModel;

namespace Visitz.Pages;

public abstract partial class VisitzPage(VisitzViewModel visitzViewModel) : ContentPage() 
{
    protected VisitzViewModel ViewModel { get; set; } = visitzViewModel;

    protected Window CurrentWindow => Window ?? GetParentWindow();

    public IDictionary<string, object> Parameters { get; set; }

    protected virtual void OnCreated() 
    {
        ConsoleTrace.TraceMethod(this);

        ViewModel.Create();
        ViewModel.AttachToLifecycle(CurrentWindow);
    }

    protected override void OnAppearing()
    {
        ConsoleTrace.TraceMethod(this);

        base.OnAppearing();
        ViewModel.Start();
    }

    protected override void OnDisappearing()
    {
        ConsoleTrace.TraceMethod(this);

        ViewModel.Stop();
        base.OnDisappearing();
    }

    protected virtual void OnDestroyed()
    {
        ConsoleTrace.TraceMethod(this);

        Behaviors.Clear();
        ViewModel.DetachFromLifecycle(CurrentWindow);
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

    public static async Task NavigateTo<T>(
        Page fromPage = null,
        IDictionary<string, object> parameters = null, 
        bool modal = false,
        bool animated = true) where T : VisitzPage
    {
        ConsoleTrace.TraceMethod(typeof(VisitzPage), $"Navigating to {typeof(T)}");

        fromPage ??= Navigator.CurrentOpenPage ?? Navigator.CurrentOpenModal;

        var newPage = ServiceProvider.Current.GetRequiredService<T>();
        newPage.Parameters = parameters;

        if (modal)
        {
            await fromPage.Navigation.PushModalAsync(newPage, animated);
        }
        else
        {
            await fromPage.Navigation.PushAsync(newPage, animated);
        }
    }
}
