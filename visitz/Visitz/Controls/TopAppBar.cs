using Visitz.Resources.Styles;

namespace Visitz.Controls;

internal class TopAppBar : Grid
{
	private static readonly double DefaultThickness = 10;
	private static readonly Color DefaultBackgroundColor = VisitzColors.Default_Background;
	private static readonly int DefaultZIndex = 2;

	public TopAppBar() : base()
	{
		Padding = new Thickness(DefaultThickness);
		BackgroundColor = DefaultBackgroundColor;
		HeightRequest = VisitzDimensions.TopAppBarHeight;
		Shadow = VisitzShadows.RestingLevel2;
		ZIndex = DefaultZIndex;
	}
}
