using Visitz.Resources.Styles;

namespace Visitz.Controls;

internal partial class TopAppBar : Grid
{
    private static readonly Color DefaultBackgroundColor = VisitzColors.Default_Background;
    private static readonly int DefaultZIndex = 2;

    public TopAppBar()
        : base()
    {
        BackgroundColor = DefaultBackgroundColor;
        ColumnSpacing = VisitzDimensions.DefaultSpacing;
        IsClippedToBounds = true;
        MinimumHeightRequest = VisitzDimensions.TopAppBarHeight;
        Padding = VisitzDimensions.DefaultSpacing;
        RowSpacing = VisitzDimensions.DefaultSpacing;
        Shadow = VisitzShadows.RestingLevel2;
        VerticalOptions = LayoutOptions.Start;
        ZIndex = DefaultZIndex;
    }
}
