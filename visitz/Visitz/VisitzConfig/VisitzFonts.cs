using Visitz.FontIcons;

namespace Visitz.VisitzConfig
{
    public static class VisitzFonts
    {
        private static readonly string BcSansBoldFile = "bcsans_bold.ttf";
        private static readonly string BcSansBoldAlias = "BCSansBold";

        private static readonly string BcSansBoldItalicFile = "bcsans_bolditalic.ttf";
        private static readonly string BcSansBoldItalicAlias = "BCSansBoldItalic";

        private static readonly string BcSansItalicFile = "bcsans_italic.ttf";
        private static readonly string BcSansItalicAlias = "BCSansItalic";

        private static readonly string BcSansRegularFile = "bcsans_regular.ttf";
        private static readonly string BcSansRegularAlias = "BCSansRegular";

        private static readonly string FontAwesome6Regular400File = "fa_6_free_regular_400.otf";
        private static readonly string FontAwesome6Regular400Alias = "FontAwesome6Regular";

        private static readonly string FontAwesome6Solid900File = "fa_6_free_solid_900.otf";
        private static readonly string FontAwesome6Solid900Alias = "FontAwesome6Solid";

        public static MauiAppBuilder ConfigureVisitzFonts(this MauiAppBuilder builder)
        {
            return builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont(BcSansBoldFile, BcSansBoldAlias);
                fonts.AddFont(BcSansBoldItalicFile, BcSansBoldItalicAlias);
                fonts.AddFont(BcSansItalicFile, BcSansItalicAlias);
                fonts.AddFont(BcSansRegularFile, BcSansRegularAlias);
                fonts.AddFont(FontAwesome6Regular400File, FontAwesome6Regular400Alias);
                fonts.AddFont(FontAwesome6Solid900File, FontAwesome6Solid900Alias);
                fonts.AddFont(MaterialIcons.RoundedFilled.Filepath, MaterialIcons.RoundedFilled.FontFamily);
                fonts.AddFont(MaterialIcons.RoundedUnfilled.Filepath, MaterialIcons.RoundedUnfilled.FontFamily);
                fonts.AddFont(FluentIcons.FontConfig.Filepath, FluentIcons.FontConfig.FontFamily);
            });
        }
    }
}
