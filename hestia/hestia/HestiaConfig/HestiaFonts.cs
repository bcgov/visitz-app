using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hestia.HestiaConfig
{
    public class HestiaFonts
    {
        private static readonly string BcSansBoldFile = "BCSans-Bold.ttf";
        private static readonly string BcSansBoldAlias = "BCSansBold";

        private static readonly string BcSansBoldItalicFile = "BCSans-BoldItalic.ttf";
        private static readonly string BcSansBoldItalicAlias = "BCSansBoldItalic";

        private static readonly string BcSansItalicFile = "BCSans-Italic.ttf";
        private static readonly string BcSansItalicAlias = "BCSansItalic";

        private static readonly string BcSansRegularFile = "BCSans-Regular.ttf";
        private static readonly string BcSansRegularAlias = "BCSansRegular";

        public static MauiAppBuilder ConfigureHestiaFonts(MauiAppBuilder builder)
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
