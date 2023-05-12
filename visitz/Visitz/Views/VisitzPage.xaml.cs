namespace Visitz.Views;

public partial class VisitzPage : ContentPage
{
    protected bool DidViewAppear;

    public VisitzPage() : base()
    {
        InitializeComponent();
    }

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
    protected virtual void OnLoad() { }
}
