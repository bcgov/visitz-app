using Visitz.VisualStates;

namespace Visitz.Views.Navigation;

public partial class NavDrawerItemView : ContentView, ISelectedState
{
    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(NavItemView));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set
        {
            SetValue(IsSelectedProperty, value);
            VisualStateManager.GoToState(this, (this as ISelectedState).GetSelectedState());
        }
    }

    public NavDrawerItemView()
    {
        InitializeComponent();
    }
}
