using Visitz.FontIcons;

namespace Visitz.VisitzConfig;

public static class VisitzFonts
{
    private static readonly string BcSansBoldFile = "bcsans_bold.ttf";
    public static readonly string BcSansBoldAlias = "BCSansBold";

    private static readonly string BcSansBoldItalicFile = "bcsans_bolditalic.ttf";
    public static readonly string BcSansBoldItalicAlias = "BCSansBoldItalic";

    private static readonly string BcSansItalicFile = "bcsans_italic.ttf";
    public static readonly string BcSansItalicAlias = "BCSansItalic";

    private static readonly string BcSansRegularFile = "bcsans_regular.ttf";
    public static readonly string BcSansRegularAlias = "BCSansRegular";

    public static MauiAppBuilder ConfigureVisitzFonts(this MauiAppBuilder builder)
    {
        return builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont(BcSansBoldFile, BcSansBoldAlias);
            fonts.AddFont(BcSansBoldItalicFile, BcSansBoldItalicAlias);
            fonts.AddFont(BcSansItalicFile, BcSansItalicAlias);
            fonts.AddFont(BcSansRegularFile, BcSansRegularAlias);
            fonts.AddFont(MaterialIcons.RoundedFilled.Filepath, MaterialIcons.RoundedFilled.FontFamily);
            fonts.AddFont(MaterialIcons.RoundedUnfilled.Filepath, MaterialIcons.RoundedUnfilled.FontFamily);
            fonts.AddFont(FluentIcons.FontConfig.Filepath, FluentIcons.FontConfig.FontFamily);
        });
    }
}
