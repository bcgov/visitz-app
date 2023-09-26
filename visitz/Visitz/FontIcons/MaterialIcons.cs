namespace Visitz.FontIcons;

public static partial class MaterialIcons
{
    public readonly struct RoundedFilled
    {
        public static readonly string Filepath = "material_icons_rounded_filled.woff2";
        public static readonly string FontFamily = "MaterialIconsRoundedFilled";
    }

    public readonly struct RoundedUnfilled
    {
        public static readonly string Filepath = "material_icons_rounded_unfilled.woff2";
        public static readonly string FontFamily = "MaterialIconsRoundedUnfilled";
    }

    public static ImageSource GetFilledMaterialIcon(this string glyph, Color color = null)
    {
        return IconStringToImageSource.Make(glyph, RoundedFilled.FontFamily, color);
    }

    public static ImageSource GetUnfilledMaterialIcon(this string glyph, Color color = null)
    {
        return IconStringToImageSource.Make(glyph, RoundedUnfilled.FontFamily, color);
    }
}