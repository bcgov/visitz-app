namespace Visitz.Views.Navigation;

public partial class NavItemView : ContentView
{
	public static readonly BindableProperty OrientationProperty =
		BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(NavItemView),
			defaultValue: StackOrientation.Vertical);

	public StackOrientation Orientation
	{
		get => (StackOrientation)GetValue(OrientationProperty);
		set => SetValue(OrientationProperty, value);
	}

	public NavItemView()
	{
		InitializeComponent();
	}
}