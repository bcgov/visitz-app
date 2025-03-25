using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;
using VisitzModel.Models.EntityTypes;
using Microsoft.Extensions.Logging;
using VisitzModel.Models.Attachments;
using Visitz.Storage;
using Realms;

namespace Visitz.Services.Attachments;
internal class GetPartialAttachmentsByRangeDownloadService(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    ILogger<GetPartialAttachmentsByRangeDownloadService> logger)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler, logger)
{
    static readonly int DefaultLimit = 10;

    public static string MakeId()
    {
        return nameof(GetPartialAttachmentsByRangeDownloadService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> items)
    {
        return new StartServiceMessage
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetPartialAttachmentsByRangeDownloadService),
            Payload = items
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    // protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    // {
    //     await ProcessAttachmentsAsync(serviceHandler, item);
    // }
    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 1
        };

        await Parallel.ForEachAsync([item], options, async (recordInfo, token) =>
        {
            await ProcessAttachmentsAsync(serviceHandler, recordInfo);
        });
    }

    private static async Task ProcessAttachmentsAsync(ServiceHandler serviceHandler, RecordServiceInfo recordInfo)
    {
        var allAttachments = await FetchAllAttachments(recordInfo);
        var filteredAttachments = FilterAndTransformAttachments(allAttachments, recordInfo);

        if (filteredAttachments.Any())
        {
            await FetchAttachmentContents(serviceHandler, filteredAttachments);
        }
    }

    private static async Task<IEnumerable<Attachment>> FetchAllAttachments(RecordServiceInfo recordInfo)
    {
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();
        var attachments = realm
        .All<Attachment>()
        .Freeze()
        .AsEnumerable()
        .Where(item => item.RelatedEntityType == recordInfo.Type
            && item.RelatedEntityId == recordInfo.Id)
        .ToList();

        return attachments;
    }

    private static IEnumerable<(EntityType entityType, string id, string attachmentId, bool force, string firstName, string lastName)> FilterAndTransformAttachments(IEnumerable<Attachment> attachments, RecordServiceInfo recordInfo)
    {
        var currentDate = DateTime.Now;

        return attachments
            .Where(att => att.UpdatedDate > DateTimeOffset.Now.AddMonths(-3))
            .Take(DefaultLimit)
            .Select(att => (
                att.RelatedEntityType,
                att.RelatedEntityId,
                att.Id,
                false,
                recordInfo.FirstName,
                recordInfo.LastName
            ));
    }
    // private async Task GetAllAttachmentContentsAsync()
    // {
    //     var options = new ParallelOptions
    //     {
    //         MaxDegreeOfParallelism = 1
    //     };

    //     await Parallel.ForEachAsync(AttachmentContentItems, options, FetchAttachmentContents);
    // }

    private static async Task FetchAttachmentContents(ServiceHandler serviceHandler, IEnumerable<(EntityType, string, string, bool, string, string)> filteredAttachments)
    {
        var getAttachmentContentServiceMessage = GetAttachmentContentByRangeService.MakeStartMessage(filteredAttachments);

        await serviceHandler.TryRunServiceAsync(getAttachmentContentServiceMessage);
    }

    protected override Exception MakePartialException(List<ApiRangeItemException<RecordServiceInfo>> exceptions)
    {
        return exceptions.CombineIntoException();
    }
}


