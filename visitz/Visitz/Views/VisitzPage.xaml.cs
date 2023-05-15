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
}
