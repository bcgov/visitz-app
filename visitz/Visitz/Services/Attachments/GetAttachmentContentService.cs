using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using Visitz.Views.Debugging;
using VisitzApi;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

internal class GetAttachmentContentService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    private (RecordServiceInfo, string, bool) AttachmentDetailsItem => ((RecordServiceInfo, string, bool))Payload;

    public static string MakeId(EntityType type, string id, string attachmentId)
    {
        return $"{nameof(GetAttachmentContentService)}|{type}|{id}|{attachmentId}";
    }

    public static StartServiceMessage MakeStartMessage(
        (RecordServiceInfo recordServiceInfo, string attachmentId, bool force) tuple
    )
    {
        return new()
        {
            ServiceId = MakeId(tuple.recordServiceInfo.Type, tuple.recordServiceInfo.Id, tuple.attachmentId),
            ServiceType = typeof(GetAttachmentContentService),
            Payload = tuple,
        };
    }

    public override string GetId()
    {
        var (recordServiceInfo, attachmentId, _) = AttachmentDetailsItem;
        return MakeId(recordServiceInfo.Type, recordServiceInfo.Id, attachmentId);
    }

    protected override async Task RunApiServiceAsync()
    {
        await DownloadAndSaveAttachmentDetailAsync(AttachmentDetailsItem);
        ResultCode = Result.Successful;
    }

    private async Task DownloadAndSaveAttachmentDetailAsync(
        (RecordServiceInfo recordServiceInfo, string attachmentId, bool force) tuple
    )
    {
        var (recordServiceInfo, attachmentId, force) = tuple;

        var after = force
            ? null
            : LastUpdatedPrefs.Get(MakeId(recordServiceInfo.Type, recordServiceInfo.Id, attachmentId));

        var attachmentJson = await Vpi.GetAttachmentDetailsAsync(
            (ApiRecordType)recordServiceInfo.Type,
            recordServiceInfo.Id,
            attachmentId,
            after
        );

        if (attachmentJson != null)
        {
            var attachment = await SaveFile(attachmentJson, recordServiceInfo);

            await VisitzRealms.EnqueueIcmDataActionAsync(
                async (realm) => await realm.WriteAsync(() => realm.Upsert(attachment))
            );
        }
    }

    private static async Task<Attachment> SaveFile(AttachmentJson json, RecordServiceInfo recordServiceInfo)
    {
        Attachment attachment = new(json, recordServiceInfo.Id, recordServiceInfo.Type);
        var attachmentFiler = await VisitzFiles.GetAsync(
            recordServiceInfo.Type,
            recordServiceInfo.Id,
            recordServiceInfo.FirstName,
            recordServiceInfo.LastName
        );

        if (DebugOptions.Default.RequireAttachmentFileContent || !string.IsNullOrWhiteSpace(json.AttachmentId))
            await VisitzFiles.EnqueueAsync(async () =>
                attachment.RelativePath = await attachmentFiler.SaveFileAsync(json.AttachmentId, json.FileExt)
            );

        return attachment;
    }
}
