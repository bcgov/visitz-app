using Visitz.VisitzConfig;

namespace Visitz.Views.TagViews;

public partial class TagView : ContentView
{
    static readonly BindableProperty.BindingPropertyChangedDelegate TagPropertyChanged = (
        boundObj,
        oldValue,
        newValue
    ) =>
    {
        (boundObj as TagView)?.UpdateUI();
    };

    static readonly BindableProperty.BindingPropertyChangedDelegate SetBorderThickness = (
        boundObj,
        oldValue,
        newValue
    ) =>
    {
        var tag = (TagView)boundObj;

        tag.Border.StrokeThickness = tag.BorderColor == Colors.Transparent ? 0.0 : tag.StrokeThickness;
    };

    static readonly BindableProperty.BindingPropertyChangedDelegate SetIconImageSourceSize = (
        boundObj,
        oldValue,
        newValue
    ) =>
    {
        var tag = (TagView)boundObj;

        if (tag.ImageSource is FontImageSource fontIcon)
            // Helps with scaling issues and makes the ImageSource look less fuzzy.
            fontIcon.Size = ((double)newValue) * 2;
    };

    public static new readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
        nameof(BackgroundColor),
        typeof(Color),
        typeof(TagView)
    );

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(TagView),
        propertyChanged: (boundObj, oldVal, newVal) =>
        {
            var tag = (TagView)boundObj;

            if (tag.ImageSource is FontImageSource fontIcon)
                fontIcon.Color = (Color)newVal;
        }
    );

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(TagView),
        defaultValue: null,
        propertyChanged: (boundObj, oldVal, newVal) =>
        {
            var tag = (TagView)boundObj;

            tag.Icon.IsVisible = newVal != null;

            if (tag.ImageSource is FontImageSource fontIcon)
                fontIcon.Color = tag.TextColor;
        }
    );

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(TagView),
        propertyChanged: TagPropertyChanged
    );

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(TagView),
        propertyChanged: SetBorderThickness
    );

    public static readonly BindableProperty IconHeightRequestProperty = BindableProperty.Create(
        nameof(IconHeightRequest),
        typeof(double),
        typeof(TagView),
        propertyChanged: SetIconImageSourceSize
    );

    public static readonly BindableProperty IconWidthRequestProperty = BindableProperty.Create(
        nameof(IconWidthRequest),
        typeof(double),
        typeof(TagView),
        propertyChanged: SetIconImageSourceSize
    );

    public static readonly BindableProperty TextTransformProperty = BindableProperty.Create(
        nameof(TextTransform),
        typeof(TextTransform),
        typeof(TagView)
    );

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(TagView),
        defaultValue: 14.0d
    );

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(TagView),
        defaultValue: VisitzFonts.BcSansRegularAlias
    );

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(TagView),
        defaultValue: new CornerRadius(20.0)
    );

    public static new readonly BindableProperty PaddingProperty = BindableProperty.Create(
        nameof(Padding),
        typeof(Thickness),
        typeof(TagView),
        defaultValue: new Thickness(5.0)
    );

    public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(
        nameof(StrokeThickness),
        typeof(double),
        typeof(TagView),
        propertyChanged: SetBorderThickness,
        defaultValue: 0.5
    );

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

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
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

    public double IconHeightRequest
    {
        get => (double)GetValue(IconHeightRequestProperty);
        set => SetValue(IconHeightRequestProperty, value);
    }

    public double IconWidthRequest
    {
        get => (double)GetValue(IconWidthRequestProperty);
        set => SetValue(IconWidthRequestProperty, value);
    }

    public TextTransform TextTransform
    {
        get => (TextTransform)GetValue(TextTransformProperty);
        set => SetValue(TextTransformProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public new Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public TagView()
    {
        InitializeComponent();
    }

    private void UpdateUI()
    {
        TagLabel.IsVisible = TagLabel.Text?.Length > 0;
    }
}
