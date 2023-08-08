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
        ViewModel.PageCreated();
        ViewModel.SubscribeToWindow(CurrentWindow);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.PageStarted();
    }

    protected override void OnDisappearing()
    {
        ViewModel.PageStopped();
        base.OnDisappearing();
    }

    protected virtual void OnDestroyed()
    {
        ViewModel.UnsubscribeFromWindow(CurrentWindow);
        ViewModel.PageDestroyed();
    }

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        var isCreating = args.OldParent == null && args.NewParent != null;
        var isDestroying = args.OldParent != null && args.NewParent == null;

        if (isCreating)
            OnCreated();
        else if (isDestroying)
            OnDestroyed();
    }

    public static async Task NavigateTo<T>(Page fromPage,
        IDictionary<string, object> parameters = null, bool modal = false,
        bool animated = true) where T : VisitzPage
    {
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
