using Visitz.ViewModels;

namespace Visitz.Pages;

public abstract partial class VisitzPage : ContentPage 
{
    protected VisitzViewModel ViewModel { get; set; }

    protected Window CurrentWindow => Window ?? GetParentWindow();

    public IDictionary<string, object> Parameters { get; set; }

    public VisitzPage(VisitzViewModel visitzViewModel) : base()
    {
        ViewModel = visitzViewModel;
        visitzViewModel.VisitzPage = this;
    }

    protected virtual void OnCreated() 
    {
        ConsoleTrace.TraceMethod(this);

        ViewModel.PageCreated();
        ViewModel.AttachToLifecycle(CurrentWindow);
    }

    protected override void OnAppearing()
    {
        ConsoleTrace.TraceMethod(this);

        base.OnAppearing();
        ViewModel.PageStarted();
    }

    protected override void OnDisappearing()
    {
        ConsoleTrace.TraceMethod(this);

        ViewModel.PageStopped();
        base.OnDisappearing();
    }

    protected virtual void OnDestroyed()
    {
        ConsoleTrace.TraceMethod(this);

        Behaviors.Clear();
        ViewModel.DetachFromLifecycle(CurrentWindow);
        ViewModel.PageDestroyed();
    }

    protected override bool OnBackButtonPressed()
    {
        ConsoleTrace.TraceMethod(this);
        return base.OnBackButtonPressed();
    }

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);
        // TODO: Implement ParentChangingEventArgsExtension here
        var isCreating = args.OldParent == null && args.NewParent != null;
        var isDestroying = args.OldParent != null && args.NewParent == null;

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

        fromPage ??= VisitzApp.CurrentOpenPage ?? VisitzApp.CurrentOpenModal;

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
