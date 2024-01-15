namespace Visitz.Views.FormControls;

public partial class FormCheckBox : ContentView
{
	public static readonly BindableProperty IsCheckedProperty =
		BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(FormCheckBox),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FormCheckBox));

    public bool IsChecked
	{
		get => (bool)GetValue(IsCheckedProperty);
		set => SetValue(IsCheckedProperty, value);
	}

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public FormCheckBox()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		IsChecked = !IsChecked;
    }
}