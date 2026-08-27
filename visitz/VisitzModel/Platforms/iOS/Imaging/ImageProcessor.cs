using Microsoft.Maui.Graphics.Platform;
using VisitzModel.Utilities;

namespace VisitzModel.Imaging;

public partial class ImageProcessor
{
    partial void DownsizeByFilesize(int bytesLength, Action<Task<Stream>> provideTaskAction)
    {
        if (bytesLength >= ImageBytes.Length)
        {
            provideTaskAction(Task.FromResult(ImageBytes));
        }
        else
        {
            ImageBytes.Seek(0, SeekOrigin.Begin);

            provideTaskAction(
                Task.Run(async () =>
                {
                    var image = PlatformImage.FromStream(ImageBytes, ImageFormat.Jpeg);
                    var newMax = ResizeImageValues.MaxNewDimensionByFileSize(image.Width, image.Height, bytesLength);

                    return image.Downsize(newMax).AsStream();
                })
            );
        }
    }

    async partial void Downsize(int maxWidthOrHeight, Action<Task<Stream>> provideTaskAction)
    {
        provideTaskAction(
            Task.Run(() =>
            {
                var image = PlatformImage.FromStream(ImageBytes, ImageFormat.Jpeg);
                var maxImageDimension = Math.Max(image.Width, image.Height);

                return maxWidthOrHeight < maxImageDimension ? image.Downsize(maxWidthOrHeight).AsStream() : ImageBytes;
            })
        );
    }
}
