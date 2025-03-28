using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Attachments;
using Visitz.Storage;
using Realms;

namespace Visitz.Services.Attachments;
internal class GetPartialAttachmentsByRangeDownloadService(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler)
    : VisitzApiService(vpi, prefs)
{
    static readonly int DefaultLimit = 10;

    static readonly int DefaultMonthLimit = 3;

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

    IEnumerable<RecordServiceInfo> Items => (IEnumerable<RecordServiceInfo>)Payload;

    protected override async Task RunApiServiceAsync()
        {
            foreach (var item in Items)
                await ProcessAttachmentsAsync(serviceHandler, item);
        }

    private static async Task ProcessAttachmentsAsync(ServiceHandler serviceHandler, RecordServiceInfo recordInfo)
    {
        var allAttachments = await FetchAllAttachments(recordInfo);
        var filteredAttachments = FilterAndTransformAttachments(allAttachments, recordInfo);

        if (filteredAttachments.Any())
            await FetchAttachmentContents(serviceHandler, filteredAttachments);
    }

    private static async Task<IEnumerable<Attachment>> FetchAllAttachments(RecordServiceInfo recordInfo)
    {
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();
        var attachments = realm.All<Attachment>().Freeze().AsEnumerable()
            .Where(item => item.RelatedEntityType == recordInfo.Type
                && item.RelatedEntityId == recordInfo.Id)
            .ToList();

        return attachments;
    }

    private static IEnumerable<
        (EntityType entityType,
        string id,
        string attachmentId,
        bool force,
        string firstName,
        string lastName)
    > FilterAndTransformAttachments(IEnumerable<Attachment> attachments, RecordServiceInfo recordInfo)
    {
        return attachments
            .Where(att => att.UpdatedDate > DateTimeOffset.Now.AddMonths(-DefaultMonthLimit))
            .Take(DefaultLimit)
            .Select(att => (
                att.RelatedEntityType,
                att.RelatedEntityId,
                att.Id,
                false,
                recordInfo.FirstName,
                recordInfo.LastName
            ))
            .ToList();
    }

    private static async Task FetchAttachmentContents(
        ServiceHandler serviceHandler,
        IEnumerable<(EntityType, string, string, bool, string, string)> filteredAttachments)
    {
        var getAttachmentContentServiceMessage = GetAttachmentContentByRangeService.MakeStartMessage(filteredAttachments);
        await serviceHandler.TryRunServiceAsync(getAttachmentContentServiceMessage);
    }
}


