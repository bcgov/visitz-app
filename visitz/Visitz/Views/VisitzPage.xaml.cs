namespace Visitz.Views;

public abstract partial class VisitzPage : ContentPage
{
    protected bool DidViewAppear;

    public VisitzPage() : base() {}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!DidViewAppear)
        {
            OnLoad();
        }
        DidViewAppear = true;
    }

    /// <summary>
    /// Subclasses can benefit by overriding this method which gets invoked once unlike `OnAppearing`
    /// </summary>
    protected abstract void OnLoad();
}
