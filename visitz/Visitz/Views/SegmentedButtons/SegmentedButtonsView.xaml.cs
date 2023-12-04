using Microsoft.Maui.Graphics.Text;

namespace Visitz.Views.SegmentedButtons;

public partial class SegmentedButtonsView : BaseContentView
{
	public static readonly BindableProperty OptionsProperty = 
		BindableProperty.Create(nameof(Options), typeof(IEnumerable<SegmentedOptions>), typeof(SegmentedButtonsView));

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(SegmentedButtonsView));

    public static readonly BindableProperty ActivatedBackgroundColorProperty =
        BindableProperty.Create(nameof(ActivatedBackgroundColor), typeof(Color), typeof(SegmentedButtonsView));

    public static readonly BindableProperty ActivatedTextColorProperty =
        BindableProperty.Create(nameof(ActivatedTextColor), typeof(Color), typeof(SegmentedButtonsView));

    public IEnumerable<SegmentedOptions> Options
	{
		get => (IEnumerable<SegmentedOptions>)GetValue(OptionsProperty);
		set => SetValue(OptionsProperty, value);
	}

	public Color Color
	{
		get => (Color)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}

    public Color ActivatedBackgroundColor
    {
        get => (Color)GetValue(ActivatedBackgroundColorProperty);
        set => SetValue(ActivatedBackgroundColorProperty, value);
    }

    public Color ActivatedTextColor
    {
        get => (Color)GetValue(ActivatedTextColorProperty);
        set => SetValue(ActivatedTextColorProperty, value);
    }

    public SegmentedButtonsView()
	{
		InitializeComponent();
	}
}