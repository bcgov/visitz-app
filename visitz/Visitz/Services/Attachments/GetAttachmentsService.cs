using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

#nullable enable

internal class GetAttachmentsService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    public static string MakeId(EntityType type, string id)
    {
        return $"{nameof(GetAttachmentsService)}|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetAttachmentsService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task RunApiServiceAsync()
    {
        Pagination pagination = new();
        int total = await DownloadAndSaveAttachmentsAsync(pagination);

        if (total > pagination.PageSize)
            await Task.WhenAll(UnrollPagination(
                total,
                pagination.PageSize,
                DownloadAndSaveAttachmentsAsync));

        ResultCode = Result.Successful;
    }

    async Task<int> DownloadAndSaveAttachmentsAsync(Pagination? pagination = null)
    {
        var (total, attachments) = await Vpi.GetAttachmentsAsync(
            (ApiRecordType)Info.Type,
            Info.Id,
            pagination);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await Attachment.SaveAttachmentsAsync(realm, attachments, Info.Id, Info.Type));

        return total;
    }
}
