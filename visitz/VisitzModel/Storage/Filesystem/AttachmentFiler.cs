using System.Globalization;
using VisitzModel.Encryption;
using VisitzModel.Formats;
using VisitzModel.Models;

namespace VisitzModel.Storage.Filesystem;

public class AttachmentFiler(CaseloadItem caseloadItem, byte[] key) : ICaseloadItemHolder
{
	public static readonly string BasePath = "Attachments";

	readonly Crypto cryptoHandler = new(key);

	public CaseloadItem CaseloadItem { get; set; } = caseloadItem;

	string CaseloadItemId => $"{CaseloadItem.EntityType}_{CaseloadItem.CaseIncidentNumber}";

	string ContextualName => CaseloadItem.KeyPlayer != null
			? $"{CaseloadItem.KeyPlayer.LastName}_{CaseloadItem.KeyPlayer.FirstName}"
			: CaseloadItemId;

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

	async Task<string> WriteEncryptedFile(Stream stream, string path, string extension)
	{
		Directory.CreateDirectory(path);

		string obfuscatedName = GenerateUniqueName(path, extension);
		await cryptoHandler.EncryptToFileAsync(stream, Path.Join(path, obfuscatedName));

		return Path.Join(path, obfuscatedName);
	}

	/// <summary>
	/// Takes in a stream, encrypts, and saves it to AppAdata as an encrypted file with the provided extension.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="filename"></param>
	/// <returns>Full path of the encrypted file that was created.</returns>
	public async Task<string> SaveFileAsync(Stream stream, string extension)
	{
		return await WriteEncryptedFile(stream, AppDataPath, extension);
	}

	/// <summary>
	/// Reads and decrypts a file from the requested full filepath.
	/// </summary>
	/// <param name="fullpath"></param>
	/// <param name="token"></param>
	/// <returns></returns>
	public async Task<MemoryStream> GetAppDataFileAsync(string fullpath, CancellationToken? token = null)
	{
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
