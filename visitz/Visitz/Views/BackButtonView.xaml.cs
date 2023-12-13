using System.Windows.Input;

namespace Visitz.Views;

[Obsolete(
    "TODO: Replace this with a regular Button with image and text once " +
    "https://github.com/dotnet/maui/issues/18924 is released"
)]
public partial class BackButtonView : ContentView
{
	public static readonly BindableProperty TextProperty 
		= BindableProperty.Create(nameof(Text), typeof(string), typeof(BackButtonView));

    public static readonly BindableProperty StrokeColorProperty
        = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(BackButtonView));

    public static readonly BindableProperty StrokeThicknessProperty
        = BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(BackButtonView));

    public static readonly BindableProperty CornerRadiusProperty
        = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(BackButtonView));

    public static readonly BindableProperty CommandProperty 
        = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(BackButtonView), null);

    public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

    public Color StrokeColor
    {
        get => (Color)GetValue(StrokeColorProperty);
        set => SetValue(StrokeColorProperty, value);
    }

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public BackButtonView()
	{
		InitializeComponent();
	}
}