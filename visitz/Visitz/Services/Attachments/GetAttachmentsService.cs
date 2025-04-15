using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

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
        await DownloadAndSaveAttachmentsAsync();
        ResultCode = Result.Successful;
    }

    async Task DownloadAndSaveAttachmentsAsync()
    {
        var attachments = await Vpi.GetAttachmentsAsync((ApiRecordType)Info.Type, Info.Id, after: null);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await Attachment.SaveAttachmentsAsync(realm, attachments, Info.Id, Info.Type));
    }
}
