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

	static string AppDataPath => Path.Combine(FileSystem.AppDataDirectory, BasePath);

	public static string MakeTimestamp() => DateTimeOffset.Now
		.ToString(IcmDateFormats.ImageTimestamp, CultureInfo.InvariantCulture);

	public string MakeFilename(string prepend, string extension)
	{
		return $"{prepend}_{ContextualName}_{MakeTimestamp()}.{extension.Trim('.')}";
	}

	async Task<string> WriteEncryptedFile(Stream stream, string fullpath)
	{
		Directory.CreateDirectory(new FileInfo(fullpath).DirectoryName);

		await cryptoHandler.EncryptToFileAsync(stream, fullpath);

		return fullpath;
	}

	/// <summary>
	/// Takes in a stream, encrypts, and saves it to AppAdata as an encrypted file with the provided extension.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="filename"></param>
	/// <returns>Full path of the encrypted file that was created.</returns>
	public async Task<string> SaveFileAsync(Stream stream, string filename)
	{
		return await WriteEncryptedFile(stream, Path.Join(AppDataPath, filename));
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
}
