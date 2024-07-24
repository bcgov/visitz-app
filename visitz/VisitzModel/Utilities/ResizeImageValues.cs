namespace VisitzModel.Utilities;

internal static class ResizeImageValues
{
	public static (float newWidth, float newHeight) ResizeByFileSize(
		float currentWidth,
		float currentHeight,
		long desiredFileSize)
	{
		return (desiredFileSize / currentHeight, desiredFileSize / currentWidth);
	}
}
