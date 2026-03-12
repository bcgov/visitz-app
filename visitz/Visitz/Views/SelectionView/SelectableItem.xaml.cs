using Visitz.VisualStates;

namespace Visitz.Views.SelectionView;

public partial class SelectableItem : ContentView, ISelectedState
{
    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(SelectableItem));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set
        {
            SetValue(IsSelectedProperty, value);
            VisualStateManager.GoToState(this, (this as ISelectedState).GetSelectedState());
        }
    }

    public SelectableItem()
    {
        InitializeComponent();
    }
}
