using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Storage;

internal static class VisitzFiles
{
	static readonly string DefaultFilesKeyName = "DefaultFilesKey";
	static readonly int Aes256KeySize = 32;

    public static async Task<AttachmentFiler> GetAsync(
        EntityType entityType,
        string caseIncidentNumber,
        string firstName,
        string lastName,
        string keyName = null)
	{
		keyName ??= DefaultFilesKeyName;
		return new AttachmentFiler(
			entityType,
			caseIncidentNumber,
			firstName,
			lastName,
			await VisitzKey.GetKey(keyName, Aes256KeySize));
	}
}
