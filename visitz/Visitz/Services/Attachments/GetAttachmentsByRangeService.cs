using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

#nullable enable

internal class GetAttachmentsByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler)
{
    public static string MakeId()
    {
        return nameof(GetAttachmentsByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetAttachmentsByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        await serviceHandler.TryRunServiceAsync(GetAttachmentsService.MakeStartMessage(item));
    }
}
