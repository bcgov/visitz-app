using Visitz.ViewModels;

namespace Visitz.Views;

public abstract partial class VisitzPage : ContentPage 
{
    protected bool HasAppeared;

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

    public static async Task NavigateTo<T>(Page fromPage, IDictionary<string, object> parameters = null) where T : VisitzPage
    {
        var newPage = VisitzApp.VisitzServices.GetRequiredService<T>();
        newPage.Parameters = parameters;
        await fromPage.Navigation.PushAsync(newPage);
    }
}
