namespace VisitzModel.Imaging;

public partial class ImageProcessor(Stream imageBytes)
{
	Stream ImageBytes { get; } = imageBytes;

	public Task<Stream> DownsizeByFilesize(int desiredMaxBytes)
	{
		if (ImageBytes.Length <= desiredMaxBytes)
			return Task.FromResult(ImageBytes);

		Task<Stream> task = default;
		DownsizeByFilesize(ref task, desiredMaxBytes);
		return task;
	}

	public Task<Stream> Downsize(int maxWidthOrHeight)
	{
		Task<Stream> task = default;
		Downsize(ref task, maxWidthOrHeight);
		return task;
	}

	partial void DownsizeByFilesize(ref Task<Stream> downsizeImageTask, int bytesLength);

	partial void Downsize(ref Task<Stream> downsizeImageTask, int maxWidthOrHeight);
}
