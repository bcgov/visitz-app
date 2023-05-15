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

    public void SubscribeToWindow(Window window)
    {
        if (window != null)
        {
            window.Created += ViewModel.Window_Created;
            window.Activated += ViewModel.Window_Activated;
            window.Deactivated += ViewModel.Window_Deactivated;
            window.Stopped += ViewModel.Window_Stopped;
            window.Resumed += ViewModel.Window_Resumed;
        }
    }

    public void UnsubscribeFromWindow(Window window)
    {
        if (window != null)
        {
            window.Created -= ViewModel.Window_Created;
            window.Activated -= ViewModel.Window_Activated;
            window.Deactivated -= ViewModel.Window_Deactivated;
            window.Stopped -= ViewModel.Window_Stopped;
            window.Resumed -= ViewModel.Window_Resumed;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!HasAppeared)
        {
            SubscribeToWindow(CurrentWindow);
            ViewModel.PageCreated();
            ViewModel.PageStarted();
            HasAppeared = true;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnsubscribeFromWindow(CurrentWindow);
    }
}
