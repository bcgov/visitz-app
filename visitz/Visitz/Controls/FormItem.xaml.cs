namespace Visitz.Controls;

public partial class FormItem : ContentView
{
	public static readonly BindableProperty NameProperty =
		BindableProperty.Create(nameof(Name), typeof(string), typeof(FormItem));

	public static readonly BindableProperty ValueProperty =
		BindableProperty.Create(nameof(Value), typeof(string), typeof(FormItem));

	public string Name
	{
		get => (string)GetValue(NameProperty);
		set => SetValue(NameProperty, value);
	}

	public string Value
	{
		get => (string)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public FormItem()
	{
		InitializeComponent();
	}
}