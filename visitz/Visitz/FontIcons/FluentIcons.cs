namespace Visitz.FontIcons;

public static partial class FluentIcons
{
    public static class FontConfig
    {
        public static readonly string Filepath = "fluentsystemicons_resizable.ttf";
        public static readonly string FontFamily = "FluentIconsRegular";
    }

    public static ImageSource GetFluentIcon(this string glyph, Color? color = null)
    {
        return IconStringToFontImageSource.Make(glyph, FontConfig.FontFamily, color);
    }
}
