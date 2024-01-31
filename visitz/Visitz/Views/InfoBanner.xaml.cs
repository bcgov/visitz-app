namespace Visitz.Views;

public partial class InfoBanner : ContentView
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(InfoBanner));

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public InfoBanner()
	{
		InitializeComponent();
	}
}