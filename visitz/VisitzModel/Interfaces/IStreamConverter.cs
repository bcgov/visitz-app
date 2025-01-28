namespace VisitzModel.Interfaces;

public interface IStreamConverter
{
    public Task<Stream> ConvertAsync(Stream stream);
}
