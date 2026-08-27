namespace VisitzModel.Utilities;

internal static class ResizeImageValues
{
    public static (float newWidth, float newHeight) NewDimensionsByFileSize(
        float currentWidth,
        float currentHeight,
        long desiredFileSize
    )
    {
        return (desiredFileSize / currentHeight, desiredFileSize / currentWidth);
    }

    public static float MaxNewDimensionByFileSize(float currentWidth, float currentHeight, long desiredFileSize)
    {
        var (newWidth, newHeight) = NewDimensionsByFileSize(currentWidth, currentHeight, desiredFileSize);
        return Math.Max(newWidth, newHeight);
    }
}
