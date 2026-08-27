namespace VisitzModel.Extensions;

public static class StreamExtensions
{
    public static async Task<byte[]> AsBytesAsync(this Stream stream)
    {
        byte[] bytesOut = new byte[stream.Length];

        stream.Seek(0, SeekOrigin.Begin);
        await stream.ReadExactlyAsync(bytesOut.AsMemory());

        return bytesOut;
    }
}
