using Visitz.VisualStates;

namespace Visitz.Views.Navigation;

public partial class NavItemView : ContentView, ISelectedState
{
	public static readonly BindableProperty OrientationProperty =
		BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(NavItemView),
			defaultValue: StackOrientation.Vertical);

	public static readonly BindableProperty IsSelectedProperty =
		BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(NavItemView));

	public StackOrientation Orientation
	{
		get => (StackOrientation)GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	public bool IsSelected
	{
		get => (bool)GetValue(IsSelectedProperty);
		set
		{
			SetValue(IsSelectedProperty, value);
			VisualStateManager.GoToState(this, (this as ISelectedState).GetSelectedState());
		}
	}

	public NavItemView()
	{
		InitializeComponent();
	}
}
