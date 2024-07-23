namespace VisitzModel.Models;

public interface IStreamConverter
{
	public Task<Stream> ConvertAsync(Stream stream);
}
