namespace Visitz;

public partial class VisitzWindow
{
	private static readonly double InitialHeight = 800;
	private static readonly double WidthRatio = 1.5d;

	private static partial Window ApplyDefaultWindowLayout(Window window)
	{
		window.Height = InitialHeight;
		window.Width = window.Height * WidthRatio;

		return window;
	}
}
