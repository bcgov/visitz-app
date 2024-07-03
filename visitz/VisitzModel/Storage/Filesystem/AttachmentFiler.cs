using Realms;
using System.Globalization;
using VisitzModel.Formats;
using VisitzModel.Models;

namespace VisitzModel.Storage.Filesystem;

public class AttachmentFiler(string basePath, CaseloadItem caseloadItem) : ICaseloadItemHolder
{
	public static readonly string PicturesPath = "Pictures";

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

	async Task<string> WriteFile(Stream stream, string path, string prepend, string extension)
	{
		Directory.CreateDirectory(path);

		string fullpath = Path.Combine(path, MakeFilename(prepend, extension));
		await using var localFileStream = File.Create(fullpath);

		stream.Seek(0, SeekOrigin.Begin);
		await stream.CopyToAsync(localFileStream);

		return fullpath;
	}

	/// <summary>
	/// Takes in a stream and saves it to cache as a file with the provided extension.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="extension"></param>
	/// <returns>Full path of the file that was created.</returns>
	public async Task<string> CacheFileAsync(Stream stream, string prepend, string extension)
	{
		return await WriteFile(stream, CachePath, prepend, extension);
	}

	public async Task<string> SaveEncryptFileAsync(Stream stream, string prepend, string extension)
	{
		string fullpath = await WriteFile(stream, AppDataPath, prepend, extension);

		// TODO: Encrypt file

		return fullpath;
	}
}
