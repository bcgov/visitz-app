using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

#nullable enable

internal class GetAttachmentContentByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<(RecordServiceInfo, string, bool force)>(
        vpi,
        prefs,
        serviceHandler,
        maxDegreeOfParallelism: 2
    )
{
    public static string MakeId()
    {
        return nameof(GetAttachmentContentByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(
        IEnumerable<(RecordServiceInfo, string, bool force)> AttachmentContentItems
    )
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetAttachmentContentByRangeService),
            Payload = AttachmentContentItems,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(
        ServiceHandler serviceHandler,
        (RecordServiceInfo, string, bool force) tuple
    )
    {
        await ServiceHandler.TryRunServiceAsync(GetAttachmentContentService.MakeStartMessage(tuple));
    }
}
