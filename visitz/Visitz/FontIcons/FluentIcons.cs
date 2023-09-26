namespace Visitz.FontIcons;

public static partial class FluentIcons
{
    public readonly struct FontConfig
    {
        public static readonly string Filepath = "fluentsystemicons_resizable.woff2";
        public static readonly string FontFamily = "FluentIconsRegular";
    }

    public static ImageSource GetFluentIcon(this string glyph, Color color = null)
    {
        return IconStringToImageSource.Make(glyph, FontConfig.FontFamily, color);
    }
}
