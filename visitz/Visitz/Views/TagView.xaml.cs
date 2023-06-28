namespace Visitz.Views;

public partial class TagView : ContentView
{
    static readonly BindableProperty.BindingPropertyChangedDelegate TagPropertyChanged = 
        (boundObj, oldValue, newValue) =>
        {
            (boundObj as TagView).UpdateUI();
        };

    public static readonly new BindableProperty BackgroundColorProperty =
        BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(TagView));

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TagView));
    
    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(TagView), 
            propertyChanged: TagPropertyChanged);
    
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(TagView), 
            propertyChanged: TagPropertyChanged);
    
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(TagView), 
            propertyChanged: TagPropertyChanged);

    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public TagView()
    {
        InitializeComponent();
    }

    private void UpdateUI()
    {
        Icon.IsVisible = IconName?.Length > 0;
        TagLabel.IsVisible = TagLabel.Text?.Length > 0;
        Border.StrokeThickness = BorderColor is null || BorderColor == Colors.Transparent ? 0 : 1;
    }
}
