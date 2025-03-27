using System.Diagnostics;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

internal class GetAttachmentContentService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    private (EntityType, string, string, bool, string, string) AttachmentDetailsItem =>
            ((EntityType, string, string, bool, string, string))Payload;

    public static string MakeId(EntityType type, string id, string attachmentId)
    {
        return $"{nameof(GetAttachmentContentService)}|{type}|{id}|{attachmentId}";
    }

    public static StartServiceMessage MakeStartMessage((EntityType entityType, string id, string attachmentId, bool force, string firstName, string lastName) tuple)
    {
        return new()
        {
            ServiceId = MakeId(tuple.entityType, tuple.id, tuple.attachmentId),
            ServiceType = typeof(GetAttachmentContentService),
            Payload = tuple,
        };
    }

    public override string GetId()
    {
        var (entityType, recordId, attachmentId, force, firstName, lastName) = AttachmentDetailsItem;
        return MakeId(entityType, recordId, attachmentId);
    }

    protected override async Task RunApiServiceAsync()
    {
        await DownloadAndSaveAttachmentDetailAsync(AttachmentDetailsItem);
        ResultCode = Result.Successful;
    }

    private async Task DownloadAndSaveAttachmentDetailAsync((EntityType entityType, string id, string attachmentId, bool force, string firstName, string lastName) tuple)
    {
        var (entityType, recordId, attachmentId, force, firstName, lastName) = tuple;
        var attachmentFiler = await VisitzFiles.GetAsync(entityType, recordId, firstName, lastName);
        var after = force ? null : prefs.Get(MakeId(tuple.entityType, tuple.id, tuple.attachmentId));
        var attachment = await Vpi.GetAttachmentDetailsAsync((ApiRecordType)entityType, recordId, attachmentId, after);

        await VisitzRealms.EnqueueIcmDataActionAsync(async() =>
        {
            using var realm = await VisitzRealms.GetIcmDataRealmAsync();
            await Attachment.SaveAttachmentAndDetailsAsync(realm, attachment, recordId, entityType, attachmentFiler);
        });
    }
}
