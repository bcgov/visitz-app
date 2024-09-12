using Microsoft.Maui.Graphics.Platform;
using VisitzModel.Utilities;

namespace VisitzModel.Imaging;

public partial class ImageProcessor
{
	partial void DownsizeByFilesize(ref Task<Stream> downsizeImageTask, int bytesLength)
	{
		downsizeImageTask = bytesLength >= ImageBytes.Length
			? Task.FromResult(ImageBytes)
			: Task.Run(() =>
			{
				var image = PlatformImage.FromStream(ImageBytes, ImageFormat.Jpeg);
				var newMax = ResizeImageValues.MaxNewDimensionByFileSize(image.Width, image.Height, bytesLength);

				return image.Downsize(newMax).AsStream();
			});
	}

	partial void Downsize(ref Task<Stream> downsizeImageTask, int maxWidthOrHeight)
	{
		downsizeImageTask = Task.Run(() =>
		{
			var image = PlatformImage.FromStream(ImageBytes, ImageFormat.Jpeg);
			var maxImageDimension = Math.Max(image.Width, image.Height);

			return maxWidthOrHeight < maxImageDimension
				? image.Downsize(maxWidthOrHeight).AsStream()
				: ImageBytes;
		});
	}
}
