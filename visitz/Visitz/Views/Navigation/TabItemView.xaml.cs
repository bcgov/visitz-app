using Visitz.VisualStates;

namespace Visitz.Views.Navigation;

#nullable enable

public partial class TabItemView : ContentView, ISelectedState
{
    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(TabItemView));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set
        {
            SetValue(IsSelectedProperty, value);
            VisualStateManager.GoToState(this, (this as ISelectedState).GetSelectedState());
        }
    }

    public TabItemView()
    {
        InitializeComponent();
    }
}
