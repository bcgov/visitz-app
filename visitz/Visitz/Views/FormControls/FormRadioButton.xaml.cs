namespace Visitz.Views.FormControls;

public partial class FormRadioButton : ContentView
{
	public static readonly BindableProperty TextProperty =
		BindableProperty.Create(nameof(Text), typeof(string), typeof(FormRadioButton));

	public static readonly BindableProperty GroupNameProperty =
		BindableProperty.Create(nameof(GroupName), typeof(string), typeof(FormRadioButton));

	public static readonly BindableProperty ValueProperty =
		BindableProperty.Create(nameof(Value), typeof(object), typeof(FormRadioButton));

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(FormRadioButton),
            defaultBindingMode: BindingMode.TwoWay);

    public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

	public string GroupName
	{
		get => (string)GetValue(GroupNameProperty);
		set => SetValue(GroupNameProperty, value);
	}

    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public FormRadioButton()
	{
		InitializeComponent();

        RadioButton.CheckedChanged += RadioButton_CheckedChanged;
	}

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        IsChecked = true;
    }
}