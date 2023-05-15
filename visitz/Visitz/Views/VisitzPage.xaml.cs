using Visitz.ViewModels;

namespace Visitz.Views;

public abstract partial class VisitzPage : ContentPage 
{
    protected bool DidViewAppear;

    protected VisitzViewModel ViewModel { get; set; }

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
        if (!DidViewAppear)
        {
            SubscribeToWindow(Window ?? GetParentWindow());
            ViewModel.PageStarted();
            OnLoad();
        }
        DidViewAppear = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnsubscribeFromWindow(Window ?? GetParentWindow());
    }

    /// <summary>
    /// Subclasses can benefit by overriding this method which gets invoked once unlike `OnAppearing`
    /// </summary>
    protected abstract void OnLoad();
}
