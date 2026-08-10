using Visitz.FontIcons;

namespace Visitz.Controls;

internal partial class LabelIcon : Label
{
    public static readonly double DefaultSize = 24.0d;

    public LabelIcon()
        : base()
    {
        FontAutoScalingEnabled = false;
        FontFamily = MaterialIcons.RoundedUnfilled.FontFamily;
        FontSize = DefaultSize;
    }
}
