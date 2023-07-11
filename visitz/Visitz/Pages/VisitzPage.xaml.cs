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

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        var isCreating = args.OldParent == null && args.NewParent != null;
        var isDestroying = args.OldParent != null && args.NewParent == null;

        if (isCreating)
        {
            ViewModel.PageCreated();
            ViewModel.SubscribeToWindow(CurrentWindow);
        }
        else if (isDestroying)
        {
            ViewModel.UnsubscribeFromWindow(CurrentWindow);
            ViewModel.PageDestroyed();
        }
    }

    public static async Task NavigateTo<T>(Page fromPage, IDictionary<string, object> parameters = null) where T : VisitzPage
    {
        var newPage = ServiceProvider.Current.GetRequiredService<T>();
        newPage.Parameters = parameters;
        await fromPage.Navigation.PushAsync(newPage);
    }
}
