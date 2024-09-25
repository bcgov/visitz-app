using VisitzModel.Models;
using VisitzModel.Storage;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Storage;

internal static class VisitzFiles
{
	static readonly string DefaultFilesKeyName = "DefaultFilesKey";
	static readonly int Aes256KeySize = 32;

	public static async Task<AttachmentFiler> GetAsync(CaseloadItem caseloadItem, string keyName = null)
	{
		keyName ??= DefaultFilesKeyName;
		return new AttachmentFiler(caseloadItem, await VisitzKey.GetKey(keyName, Aes256KeySize));
	}
}
