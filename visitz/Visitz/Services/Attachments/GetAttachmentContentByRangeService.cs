using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Services.Attachments;

internal class GetAttachmentContentByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiService(vpi, prefs)
{
    private IEnumerable<ValueTuple<EntityType, string, string, bool, string, string>> AttachmentContentItems =>
            (IEnumerable<ValueTuple<EntityType, string, string, bool, string, string>>)Payload;

    private ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    public static string MakeId()
    {
        return nameof(GetAttachmentContentByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<ValueTuple<EntityType, string, string, bool, string, string>> AttachmentContentItems)
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

    protected override async Task RunApiServiceAsync()
    {
        await GetAllAttachmentContentsAsync();
    }

    private async Task GetAllAttachmentContentsAsync()
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 2
        };

        await Parallel.ForEachAsync(AttachmentContentItems, options, GetAttachmentContentRecord);
    }

    private async ValueTask GetAttachmentContentRecord((EntityType entityType, string id, string attachmentId, bool force, string firstName, string lastName) tuple, CancellationToken token)
    {
        _ = await ServiceHandler.TryRunServiceAsync(GetAttachmentContentService.MakeStartMessage(tuple));
    }
}
