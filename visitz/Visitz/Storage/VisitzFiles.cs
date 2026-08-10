using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;
using VisitzModel.Storage.Filesystem;
using VisitzModel.Utilities;

namespace Visitz.Storage;

internal static class VisitzFiles
{
    static readonly string DefaultFilesKeyName = "DefaultFilesKey";
    static readonly int Aes256KeySize = 32;

    static readonly EagerActionQueue eagerFilesQueue = new();

    public static async Task<AttachmentFiler> GetAsync(IBusinessObject businessObject, string? keyName = null)
    {
        return await GetAsync(
            businessObject.EntityType,
            businessObject.FileNumber,
            businessObject.GivenNames,
            businessObject.LastName,
            keyName
        );
    }

    public static async Task<AttachmentFiler> GetAsync(
        EntityType entityType,
        string caseIncidentNumber,
        string firstName,
        string lastName,
        string? keyName = null
    )
    {
        keyName ??= DefaultFilesKeyName;
        return new AttachmentFiler(
            entityType,
            caseIncidentNumber,
            firstName,
            lastName,
            await VisitzKey.GetKey(keyName, Aes256KeySize)
                ?? throw new InvalidOperationException($"Missing key for {nameof(AttachmentFiler)}")
        );
    }

    public static Task<AttachmentFiler> GetAsync(
        Attachment attachment,
        string firstName,
        string lastName,
        string? keyname = null
    )
    {
        return GetAsync(attachment.RelatedEntityType, attachment.FileNumber, firstName, lastName, keyname);
    }

    public static Task EnqueueAsync(Func<Task> task)
    {
        return eagerFilesQueue.EnqueueAsync(task);
    }
}
