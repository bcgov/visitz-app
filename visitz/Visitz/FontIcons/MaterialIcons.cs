namespace Visitz.FontIcons;

public static partial class MaterialIcons
{
    public static class RoundedFilled
    {
        public static readonly string Filepath = "material_icons_rounded_filled.ttf";
        public static readonly string FontFamily = "MaterialIconsRoundedFilled";
    }

    public static class RoundedUnfilled
    {
        public static readonly string Filepath = "material_icons_rounded_unfilled.ttf";
        public static readonly string FontFamily = "MaterialIconsRoundedUnfilled";
    }

    public static ImageSource GetFilledMaterialIcon(this string glyph, Color? color = null)
    {
        return IconStringToFontImageSource.Make(glyph, RoundedFilled.FontFamily, color);
    }

    public static ImageSource GetUnfilledMaterialIcon(this string glyph, Color? color = null)
    {
        return IconStringToFontImageSource.Make(glyph, RoundedUnfilled.FontFamily, color);
    }
}
