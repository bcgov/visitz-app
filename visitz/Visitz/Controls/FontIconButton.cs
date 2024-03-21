namespace Visitz.Controls;

public class FontIconButton : Button
{
	static readonly double DefaultDimension = 44;
	static readonly double DefaultFontSize = 24;

	public FontIconButton()
	{
		FontSize = DefaultFontSize;
		HeightRequest = DefaultDimension;
		WidthRequest = DefaultDimension;
	}
}
