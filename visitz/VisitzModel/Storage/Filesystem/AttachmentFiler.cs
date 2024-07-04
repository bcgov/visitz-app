using System.Globalization;
using VisitzModel.Encryption;
using VisitzModel.Formats;
using VisitzModel.Models;

namespace VisitzModel.Storage.Filesystem;

public class AttachmentFiler(string basePath, CaseloadItem caseloadItem, byte[] key) : ICaseloadItemHolder
{
	public static readonly string PicturesPath = "Pictures";

	readonly Crypto cryptoHandler = new(key);

	public CaseloadItem CaseloadItem { get; set; } = caseloadItem;

	string CaseloadItemId => $"{CaseloadItem.EntityType}_{CaseloadItem.CaseIncidentNumber}";

	string ContextualName => CaseloadItem.KeyPlayer != null
			? $"{CaseloadItem.KeyPlayer.LastName}_{CaseloadItem.KeyPlayer.FirstName}"
			: CaseloadItemId;

	string CachePath => Path.Combine(FileSystem.CacheDirectory, basePath, CaseloadItemId);

	string AppDataPath => Path.Combine(FileSystem.AppDataDirectory, basePath, CaseloadItemId);

	static string MakeTimestamp() => DateTimeOffset.Now
		.ToString(IcmDateFormats.ImageTimestamp, CultureInfo.InvariantCulture);

	string MakeFilename(string prepend, string extension)
	{
		return $"{prepend}_{ContextualName}_{MakeTimestamp()}.{extension.Trim('.')}";
	}

	async Task<string> WriteEncryptedFile(Stream stream, string path, string prepend, string extension)
	{
		Directory.CreateDirectory(path);

		string fullpath = Path.Combine(path, MakeFilename(prepend, extension));
		await cryptoHandler.EncryptToFileAsync(stream, fullpath);

		return fullpath;
	}

	/// <summary>
	/// Takes in a stream, encrypts, and saves it to AppAdata as an encrypted file with the provided extension.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="prepend"></param>
	/// <param name="extension"></param>
	/// <returns>Full path of the encrypted file that was created.</returns>
	public async Task<string> SaveFileAsync(Stream stream, string prepend, string extension)
	{
		return await WriteEncryptedFile(stream, AppDataPath, prepend, extension);
	}
}
