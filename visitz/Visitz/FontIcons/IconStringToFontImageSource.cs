namespace Visitz.FontIcons;

public static class IconStringToFontImageSource
{
    public static FontImageSource Make(string glyph, string fontFamily, Color? color = null)
    {
        return new FontImageSource()
        {
            Glyph = glyph,
            FontFamily = fontFamily,
            Color = color ?? Colors.Black,
        };
    }
}
