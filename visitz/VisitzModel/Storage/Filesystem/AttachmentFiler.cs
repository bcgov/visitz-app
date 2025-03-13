using System.Globalization;
using VisitzModel.Encryption;
using VisitzModel.Formats;

namespace VisitzModel.Storage.Filesystem;

public class AttachmentFiler(string EntityType, string CaseIncidentNumber, string FirstName, string LastName, byte[] key)
{
	static readonly string BasePath = "Attachments";

	readonly Crypto cryptoHandler = new(key);

	string CaseloadItemId => $"{EntityType}_{CaseIncidentNumber}";

	string ContextualName => string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName)
			? CaseloadItemId
			: $"{LastName}_{FirstName}";

	static string AppDataPath =>
#if WINDOWS
		// Store app data in its container instead of Windows' default path.
		Path.Combine(FileSystem.AppDataDirectory, BasePath);
#else
		// For non-Windows, store in Personal SpecialFolder.
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), BasePath);
#endif

	public static string MakeTimestamp() => DateTimeOffset.Now
		.ToString(IcmDateFormats.ImageTimestamp, CultureInfo.InvariantCulture);

	public string MakeFilename(string prepend, string extension)
	{
		return $"{prepend}_{ContextualName}_{MakeTimestamp()}.{extension.Trim('.')}";
	}

	public static string GetFullPath(string relativePath)
	{
		return Path.Join(AppDataPath, relativePath);
	}

	async Task<string> WriteEncryptedFile(Stream stream, string extension)
	{
		Directory.CreateDirectory(AppDataPath);

		string obfuscatedName = GenerateUniqueName(AppDataPath, extension);
		await cryptoHandler.EncryptToFileAsync(stream, Path.Join(AppDataPath, obfuscatedName));

		return obfuscatedName;
	}

	/// <summary>
	/// Takes in a stream, encrypts, and saves it to AppData as an encrypted file with the provided extension.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="extension"></param>
	/// <returns>Relative path of the encrypted file that was created.</returns>
	public async Task<string> SaveFileAsync(Stream stream, string extension)
	{
		return await WriteEncryptedFile(stream, extension);
	}

	/// <summary>
	/// Reads and decrypts a file from the requested relative filepath.
	/// </summary>
	/// <param name="relativePath"></param>
	/// <param name="token"></param>
	/// <returns><see cref="MemoryStream"/> of the decrypted file</returns>
	public async Task<MemoryStream> GetAppDataFileAsync(string relativePath, CancellationToken? token = null)
	{
		string fullpath = GetFullPath(relativePath);
		await using var cryptoStream = await cryptoHandler.DecryptFromFileAsync(fullpath);

		var memoryStream = new MemoryStream();
		await cryptoStream.CopyToAsync(memoryStream, token ?? CancellationToken.None);

		memoryStream.Seek(0, SeekOrigin.Begin);
		return memoryStream;
	}

	static string GenerateUniqueName(string path, string extension)
	{
		string uniqueName;

		do
			uniqueName = Guid.NewGuid().ToString() + extension;
		while (File.Exists(Path.Join(path, uniqueName)));

		return uniqueName;
	}
}
