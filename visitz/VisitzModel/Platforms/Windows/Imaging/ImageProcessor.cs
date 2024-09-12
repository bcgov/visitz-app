using VisitzModel.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace VisitzModel.Imaging;

public partial class ImageProcessor
{
	partial void DownsizeByFilesize(ref Task<Stream> downsizeImageTask, int bytesLength)
	{
		downsizeImageTask = bytesLength >= ImageBytes.Length
			? Task.FromResult(ImageBytes)
			: Task.Run(async () =>
			{
				var image = await ConvertToImageAsync(ImageBytes);
				var newMax = ResizeImageValues.MaxNewDimensionByFileSize(image.Width, image.Height, bytesLength);

				ResizeImage(image, (int)newMax);
				return await ConvertToStreamAsync(image);
			});
	}

	partial void Downsize(ref Task<Stream> downsizeImageTask, int maxWidthOrHeight)
	{
		downsizeImageTask = Task.Run(async () =>
		{
			var image = await ConvertToImageAsync(ImageBytes);

			if (Math.Max(image.Width, image.Height) > maxWidthOrHeight)
			{
				ResizeImage(image, maxWidthOrHeight);
				return await ConvertToStreamAsync(image);
			}
			else
				return ImageBytes;
		});
	}

	static async Task<Image> ConvertToImageAsync(Stream imageBytes, CancellationToken? token = null)
	{
		return await Image.LoadAsync(imageBytes, token ?? CancellationToken.None);
	}

	/// <summary>
	/// .NET MAUI 8.x's Microsoft.Maui.Graphics.IImage.Downsize() functionality isn't implemented on Windows so we
	/// need to use an external library for it.
	/// </summary>
	/// <param name="image"></param>
	/// <param name="maxWidthOrHeight"></param>
	static void ResizeImage(Image image, int maxWidthOrHeight)
	{
		image.Mutate(i =>
		{
			i.Resize(new ResizeOptions
			{
				Size = new SixLabors.ImageSharp.Size(maxWidthOrHeight),
				Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max,
			});
		});
	}

	static async Task<Stream> ConvertToStreamAsync(Image image)
	{
		MemoryStream outStream = new();
		await image.SaveAsJpegAsync(outStream);

		outStream.Seek(0, SeekOrigin.Begin);
		return outStream;
	}
}
