using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

internal class GetAttachmentContentByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<(RecordServiceInfo, string, bool)>(vpi, prefs, serviceHandler, maxDegreeOfParallelism: 2)
{
    private IEnumerable<(RecordServiceInfo, string, bool)> AttachmentContentItems =>
        (IEnumerable<(RecordServiceInfo, string, bool)>)Payload;

    public static string MakeId()
    {
        return nameof(GetAttachmentContentByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(
        IEnumerable<(RecordServiceInfo, string, bool)> AttachmentContentItems
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
        (RecordServiceInfo, string, bool) tuple
    )
    {
        await ServiceHandler.TryRunServiceAsync(GetAttachmentContentService.MakeStartMessage(tuple));
    }

    protected override Exception MakePartialException(
        List<ApiRangeItemException<(RecordServiceInfo, string, bool)>> exceptions
    )
    {
        var recordServiceInfoExceptions = exceptions
            .Select(ex => new ApiRangeItemException<RecordServiceInfo>(ex.Item.Item1, ex.InnerException))
            .ToList();

        return recordServiceInfoExceptions.CombineIntoException();
    }
}
