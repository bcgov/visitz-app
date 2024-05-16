using Visitz.Resources.Styles;

namespace Visitz.Controls;

internal class TopAppBar : HorizontalStackLayout
{
	private static readonly double DefaultHorizontalThickness = 5;
	private static readonly double DefaultVerticalThickness = 10;
	private static readonly Color DefaultBackgroundColor = VisitzColors.Default_Background;
	private static readonly int DefaultZIndex = 2;

	public TopAppBar() : base()
	{
		Padding = new Thickness(DefaultHorizontalThickness, DefaultVerticalThickness);
		BackgroundColor = DefaultBackgroundColor;
		HeightRequest = VisitzDimensions.TopAppBarHeight;
		Shadow = VisitzShadows.RestingLevel2;
		ZIndex = DefaultZIndex;
	}
}
