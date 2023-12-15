namespace Visitz.Views.FormControls;

public partial class FormEntry : ContentView
{
    public static readonly BindableProperty FieldNameProperty =
        BindableProperty.Create(nameof(FieldName), typeof(string), typeof(FormEntry));

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FormEntry));

    public static readonly BindableProperty LeadingSupportingTextProperty =
        BindableProperty.Create(nameof(LeadingSupportingText), typeof(string), typeof(FormEntry));

    public static readonly BindableProperty TrailingSupportingTextProperty =
        BindableProperty.Create(nameof(TrailingSupportingText), typeof(string), typeof(FormEntry));

    public string FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string LeadingSupportingText
    {
        get => (string)GetValue(LeadingSupportingTextProperty);
        set => SetValue(LeadingSupportingTextProperty, value);
    }

    public string TrailingSupportingText
    {
        get => (string)GetValue(TrailingSupportingTextProperty);
        set => SetValue(TrailingSupportingTextProperty, value);
    }

    public FormEntry()
	{
		InitializeComponent();
	}
}