using Visitz.ViewModels;

namespace Visitz.Views;

public abstract partial class VisitzPage : ContentPage 
{
    protected bool HasAppeared;

    protected VisitzViewModel ViewModel { get; set; }

    protected Window CurrentWindow => Window ?? GetParentWindow();

    public VisitzPage(VisitzViewModel visitzViewModel) : base()
    {
        ViewModel = visitzViewModel;
        visitzViewModel.VisitzPage = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!HasAppeared)
        {
            ViewModel.SubscribeToWindow(CurrentWindow);

            ViewModel.PageCreated();
            ViewModel.PageStarted();

            HasAppeared = true;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        ViewModel.UnsubscribeFromWindow(CurrentWindow);
    }

    protected static async Task NavigateTo(Type page, IDictionary<string, object> parameters)
    {
        Routing.RegisterRoute(page.Name, page);
        await Shell.Current.GoToAsync(page.Name, parameters);
    }

    protected static async Task NavigateTo(Type page)
    {
        Routing.RegisterRoute(page.Name, page);
        await Shell.Current.GoToAsync(page.Name);
    }
}
