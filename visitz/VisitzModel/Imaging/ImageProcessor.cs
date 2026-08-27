namespace VisitzModel.Imaging;

public partial class ImageProcessor(Stream imageBytes)
{
    static readonly string MaxLoopsError = "Reached loop limit ({0}) when downsizing image ({1} bytes)";

    static readonly float InitialFactor = 1.0f;

    static readonly float ReductionFactor = 0.8f;

    static readonly int FilesizeDownsizeLoopLimit = 25;

    Stream ImageBytes { get; } = imageBytes;

    public async Task<Stream> DownsizeImageByFilesize(int desiredMaxBytes)
    {
        if (ImageBytes.Length <= desiredMaxBytes)
            return ImageBytes;

        Stream stream = Stream.Null;
        float factor = InitialFactor;
        int loopCount = 0;

        do
        {
            Task<Stream> downsizeTask = Task.FromResult(Stream.Null);
            DownsizeByFilesize((int)(desiredMaxBytes * factor), workTask => downsizeTask = workTask);
            stream = await downsizeTask;

            if (stream.Length <= 0)
                throw new InvalidOperationException("Unable to downsize image");

            factor *= ReductionFactor;
            loopCount++;

            if (loopCount >= FilesizeDownsizeLoopLimit)
            {
                var error = string.Format(MaxLoopsError, FilesizeDownsizeLoopLimit, stream.Length);
                throw new InvalidOperationException(error);
            }
        } while (stream.Length > desiredMaxBytes);

        return stream;
    }

    public async Task<Stream> DownsizeImage(int maxWidthOrHeight)
    {
        Task<Stream> downsizeTask = Task.FromResult(Stream.Null);

        Downsize(maxWidthOrHeight, workTask => downsizeTask = workTask);
        Stream stream = await downsizeTask;

        if (stream.Length <= 0)
            throw new InvalidOperationException("Unable to downsize image");

        return stream;
    }

    partial void DownsizeByFilesize(int bytesLength, Action<Task<Stream>> provideTaskAction);

    partial void Downsize(int maxWidthOrHeight, Action<Task<Stream>> provideTaskAction);
}
