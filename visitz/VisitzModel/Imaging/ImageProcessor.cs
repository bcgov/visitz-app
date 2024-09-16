namespace VisitzModel.Imaging;

public partial class ImageProcessor(Stream imageBytes)
{
	static readonly string MaxLoopsError = "Reached loop limit ({0}) when downsizing image ({1} bytes)";

	static readonly float InitialFactor = 1.0f;

	static readonly float ReductionFactor = 0.8f;

	static readonly int FilesizeDownsizeLoopLimit = 25;

	Stream ImageBytes { get; } = imageBytes;

	public async Task<Stream> DownsizeByFilesize(int desiredMaxBytes)
	{
		if (ImageBytes.Length <= desiredMaxBytes)
			return ImageBytes;

		Stream streamOut;
		float factor = InitialFactor;
		int loopCount = 0;

		do
		{
			Task<Stream> task = default;
			DownsizeByFilesize(ref task, (int)(desiredMaxBytes * factor));
			streamOut = await task;

			factor *= ReductionFactor;
			loopCount++;

			if (loopCount >= FilesizeDownsizeLoopLimit)
			{
				var error = string.Format(MaxLoopsError, FilesizeDownsizeLoopLimit, streamOut.Length);
				throw new InvalidOperationException(error);
			}

		} while (streamOut.Length > desiredMaxBytes);

		return streamOut;
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
