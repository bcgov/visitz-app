using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace Visitz.Extensions;

public static class StreamImageExtensions
{
    public static IImage MakeThumbnail(
        this Stream stream,
        float maxWidthOrHeight,
        ImageFormat imageFormat = ImageFormat.Jpeg,
        bool disposeStream = false
    )
    {
        stream.Seek(0, SeekOrigin.Begin);
        return PlatformImage.FromStream(stream, imageFormat).Downsize(maxWidthOrHeight, disposeStream);
    }
}
