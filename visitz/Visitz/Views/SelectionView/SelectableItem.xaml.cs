using System.ComponentModel;
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
        PropertyChanged += SelectableItem_PropertyChanged;
	}

    private void SelectableItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
		if (e.PropertyName != nameof(Content))
			return;

		var tap = new TapGestureRecognizer() { Buttons = ButtonsMask.Primary, };
        tap.Tapped += Tap_Tapped;

        Content.GestureRecognizers.Add(tap);
    }

    private void Tap_Tapped(object sender, TappedEventArgs e)
    {
		IsSelected = !IsSelected;
    }
}