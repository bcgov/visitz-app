using System.Security.Cryptography;

namespace VisitzModel.Encryption;

internal class Crypto(byte[] key)
{
	readonly byte[] _key = key;

	public async Task EncryptToFileAsync(Stream stream, string fullpath)
	{
		using Aes aes = Aes.Create();
		aes.Key = _key;

		await using FileStream fileStream = new(fullpath, FileMode.OpenOrCreate);
		var iv = aes.IV.AsMemory();
		await fileStream.WriteAsync(iv);

		stream.Seek(0, SeekOrigin.Begin);

		CryptoStreamMode mode = CryptoStreamMode.Write & CryptoStreamMode.Read;
		await using CryptoStream cryptoStream = new(stream, aes.CreateEncryptor(), mode);

		await cryptoStream.CopyToAsync(fileStream);
	}
}
