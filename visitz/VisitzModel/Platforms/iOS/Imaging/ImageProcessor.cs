using Microsoft.Maui.Graphics.Platform;
using VisitzModel.Extensions;
using VisitzModel.Utilities;

namespace VisitzModel.Imaging;

public partial class ImageProcessor
{
	static readonly string MaxLoopsError = "Reached loop limit ({0}) when downsizing image ({1} bytes)";

	static readonly float InitialFactor = 1.0f;

	static readonly float ReductionFactor = 0.9f;

	static readonly int FilesizeDownsizeLoopLimit = 25;

	partial void DownsizeByFilesize(ref Task<Stream> downsizeImageTask, int bytesLength)
	{
		downsizeImageTask = bytesLength >= ImageBytes.Length
			? Task.FromResult(ImageBytes)
			: Task.Run(() =>
			{
				ImageBytes.Seek(0, SeekOrigin.Begin);
				var image = PlatformImage.FromStream(ImageBytes, ImageFormat.Jpeg);
				var newMax = ResizeImageValues.MaxNewDimensionByFileSize(image.Width, image.Height, bytesLength);

				Stream streamOut;
				float factor = InitialFactor;
				int loopCount = 0;

				do
				{
					streamOut = image.Downsize(newMax * factor).AsStream();
					
					factor *= ReductionFactor;
					loopCount++;

					if (loopCount >= FilesizeDownsizeLoopLimit)
					{
						var error = MaxLoopsError.Format(FilesizeDownsizeLoopLimit, streamOut.Length);
						throw new InvalidOperationException(error);
					}

				} while (streamOut.Length > bytesLength);

				return streamOut;
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
