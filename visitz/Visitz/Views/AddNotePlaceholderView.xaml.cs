namespace Visitz.Views;

public partial class AddNotePlaceholderView : ContentView
{
    public static readonly BindableProperty NotePeriodProperty =
        BindableProperty.Create(nameof(NotePeriod), typeof(string), typeof(AddNotePlaceholderView));

    public static readonly BindableProperty ShowNotePeriodProperty =
        BindableProperty.Create(nameof(ShowNotePeriod), typeof(bool), typeof(AddNotePlaceholderView));

    public static readonly BindableProperty ContentTextProperty =
        BindableProperty.Create(nameof(ContentText), typeof(string), typeof(AddNotePlaceholderView));

    public string NotePeriod
    {
        get => (string)GetValue(NotePeriodProperty);
        set => SetValue(NotePeriodProperty, value);
    }

    public bool ShowNotePeriod
    {
        get => (bool)GetValue(ShowNotePeriodProperty);
        set => SetValue(ShowNotePeriodProperty, value);
    }

    public string ContentText
    {
        get => (string)GetValue(ContentTextProperty);
        set => SetValue(ContentTextProperty, value);
    }

    public AddNotePlaceholderView()
	{
		InitializeComponent();
	}
}
