using Microsoft.Extensions.Logging;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services.Attachments;

internal class GetAttachmentContentByRangeService(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    ILogger<GetAttachmentContentByRangeService> logger)
    : VisitzApiRangeService<(EntityType, string, string, bool, string, string)>(
        vpi,
        prefs,
        serviceHandler,
        logger,
        new ParallelOptions { MaxDegreeOfParallelism = 2 })
{
    private IEnumerable<(EntityType, string, string, bool, string, string)> AttachmentContentItems =>
            (IEnumerable<(EntityType, string, string, bool, string, string)>)Payload;

    private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    public static string MakeId()
    {
        return nameof(GetAttachmentContentByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(
        IEnumerable<(EntityType, string, string, bool, string, string)> AttachmentContentItems)
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
        ServiceHandler serviceHandler, (EntityType, string, string, bool, string, string) tuple)
    {
        await ServiceHandler.TryRunServiceAsync(GetAttachmentContentService.MakeStartMessage(tuple));
    }

    protected override Exception MakePartialException(
        List<ApiRangeItemException<(EntityType, string, string, bool, string, string)>> exceptions)
    {
        return new AggregateException(exceptions);
    }
}
