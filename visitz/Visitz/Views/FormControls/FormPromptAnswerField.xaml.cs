namespace Visitz.Views.FormControls;

public partial class FormPromptAnswerField : ContentView
{
    public static readonly BindableProperty RadioButtonGroupNameProperty =
        BindableProperty.Create(nameof(RadioButtonGroupName), typeof(string), typeof(FormPromptAnswerField)); 

    public static readonly BindableProperty QuestionPromptProperty =
        BindableProperty.Create(nameof(QuestionPrompt), typeof(string), typeof(FormPromptAnswerField));

    public static readonly BindableProperty AnswerProperty =
        BindableProperty.Create(nameof(Answer), typeof(YesNoAnswer?), typeof(FormPromptAnswerField),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty AnswerContextExplanationProperty =
        BindableProperty.Create(nameof(AnswerContextExplanation), typeof(string), typeof(FormPromptAnswerField),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty AnswerContextProperty =
        BindableProperty.Create(nameof(AnswerContext), typeof(string), typeof(FormPromptAnswerField));

    public static readonly BindableProperty InlineContentProperty =
        BindableProperty.Create(nameof(InlineContent), typeof(View), typeof(FormPromptAnswerField));

    public string RadioButtonGroupName
    {
        get => (string)GetValue(RadioButtonGroupNameProperty);
        set => SetValue(RadioButtonGroupNameProperty, value);
    }

    public string QuestionPrompt
    {
        get => (string)GetValue(QuestionPromptProperty);
        set => SetValue(QuestionPromptProperty, value);
    }

    public YesNoAnswer? Answer
    {
        get => (YesNoAnswer?)GetValue(AnswerProperty);
        set => SetValue(AnswerProperty, value);
    }

    public string AnswerContextExplanation
    {
        get => (string)GetValue(AnswerContextExplanationProperty);
        set => SetValue(AnswerContextExplanationProperty, value);
    }

    public string AnswerContext
    {
        get => (string)GetValue(AnswerContextProperty);
        set => SetValue(AnswerContextProperty, value);
    }

    public View InlineContent
    {
        get => (View)GetValue(InlineContentProperty);
        set => SetValue(InlineContentProperty, value);
    }

    public FormPromptAnswerField()
	{
        RadioButtonGroupName = $"G{Guid.NewGuid():N}";

		InitializeComponent();
	}

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var rb = (RadioButton)sender;

        if (rb.IsChecked)
            Answer = (YesNoAnswer)rb.Value;
    }
}