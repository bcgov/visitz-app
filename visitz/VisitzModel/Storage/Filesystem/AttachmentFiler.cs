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

	static string MakeTimestamp() => DateTimeOffset.Now
		.ToString(IcmDateFormats.ImageTimestamp, CultureInfo.InvariantCulture);

	string MakeFilename(string prepend, string extension)
	{
		return $"{prepend}_{ContextualName}_{MakeTimestamp()}.{extension.Trim('.')}";
	}

	/// <summary>
	/// Takes in a stream and saves it to cache as a file with the provided extension.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="extension"></param>
	/// <returns>Full path of the file that was created.</returns>
	public async Task<string> CacheFile(Stream stream, string prepend, string extension)
	{
		Directory.CreateDirectory(CachePath);

		string fullpath = Path.Combine(CachePath, MakeFilename(prepend, extension));
		await using var localFileStream = File.Create(fullpath);

		await stream.CopyToAsync(localFileStream);

		return fullpath;
	}
}
