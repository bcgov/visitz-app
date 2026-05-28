using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

#nullable enable

internal class GetAttachmentsService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    readonly ConcurrentBag<AttachmentJson> _attachments = [];

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

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, attachments) = await Vpi.GetAttachmentsAsync((ApiRecordType)Info.Type, Info.Id, pagination);

        _attachments.AddAll(attachments);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await Attachment.SynchronizeAsync(realm, _attachments, Info.Id, Info.Type)
        );
    }
}
