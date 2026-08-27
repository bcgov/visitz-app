namespace Visitz.Controls;

public partial class FontIconButton : Button
{
    static readonly double DefaultDimension = 44;
    static readonly double DefaultFontSize = 24;

    public static readonly double LargerDimension = 60;
    static readonly double LargerFontSize = 34;

    public static readonly BindableProperty SizeProperty = BindableProperty.Create(
        nameof(Size),
        typeof(FontIconButtonSize),
        typeof(FontIconButton),
        FontIconButtonSize.Unknown,
        propertyChanged: (boundObj, oldVal, newVal) =>
        {
            var fiButton = (FontIconButton)boundObj;
            var size = (FontIconButtonSize)newVal;

            fiButton.HeightRequest = fiButton.WidthRequest = newVal.Equals(FontIconButtonSize.Larger)
                ? LargerDimension
                : DefaultDimension;

            fiButton.FontSize = newVal.Equals(FontIconButtonSize.Larger) ? LargerFontSize : DefaultFontSize;
        }
    );

    public FontIconButtonSize Size
    {
        get => (FontIconButtonSize)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public FontIconButton()
    {
        FontSize = DefaultFontSize;
        HeightRequest = DefaultDimension;
        WidthRequest = DefaultDimension;
    }
}

public enum FontIconButtonSize
{
    Unknown = 0,
    ToolbarItem = 1,
    Larger = 2,
}
