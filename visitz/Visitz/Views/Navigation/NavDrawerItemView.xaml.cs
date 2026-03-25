using System.Windows.Input;
using Visitz.VisualStates;

namespace Visitz.Views.Navigation;

public partial class NavDrawerItemView : ContentView, ISelectedState
{
    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(NavDrawerItemView));

    public static readonly BindableProperty TappedCommandProperty =
        BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(NavDrawerItemView));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set
        {
            SetValue(IsSelectedProperty, value);
            VisualStateManager.GoToState(this, (this as ISelectedState).GetSelectedState());
        }
    }

    public ICommand TappedCommand
    {
        get => (ICommand)GetValue(TappedCommandProperty);
        set => SetValue(TappedCommandProperty, value);
    }

    public NavDrawerItemView()
    {
        InitializeComponent();
    }

    private void SfEffectsView_TouchDown(object sender, EventArgs e)
    {
        TappedCommand?.Execute(null);
    }
}
