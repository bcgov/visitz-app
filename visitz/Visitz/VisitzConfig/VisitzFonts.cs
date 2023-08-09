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

        public static MauiAppBuilder ConfigureVisitzFonts(this MauiAppBuilder builder)
        {
            return builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont(BcSansBoldFile, BcSansBoldAlias);
                fonts.AddFont(BcSansBoldItalicFile, BcSansBoldItalicAlias);
                fonts.AddFont(BcSansItalicFile, BcSansItalicAlias);
                fonts.AddFont(BcSansRegularFile, BcSansRegularAlias);
            });
        }
    }
}
