using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;
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
        var allFilteredAttachments = new List<(RecordServiceInfo, string, bool)>();

        foreach (var item in Items)
        {
            IEnumerable<(RecordServiceInfo, string, bool)> filteredAttachments = await ProcessAttachmentsAsync(serviceHandler, item);
            allFilteredAttachments = allFilteredAttachments.Concat(filteredAttachments).ToList();
        }

        if (allFilteredAttachments.Count != 0)
        {
            await FetchAttachmentContents(serviceHandler, allFilteredAttachments);
        }
    }

    private static async Task<IEnumerable<(RecordServiceInfo recordInfo, string attachmentId, bool force)>> ProcessAttachmentsAsync(
        ServiceHandler serviceHandler, RecordServiceInfo recordInfo)
    {
        var allAttachments = await FetchAllAttachments(recordInfo);
        var filteredAttachments = FilterAndTransformAttachments(allAttachments, recordInfo);

        return filteredAttachments;
    }

    private static async Task<IEnumerable<Attachment>> FetchAllAttachments(RecordServiceInfo recordInfo)
    {
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();
        return realm.All<Attachment>().Freeze().AsEnumerable()
            .Where(item => item.RelatedEntityType == recordInfo.Type
                && item.RelatedEntityId == recordInfo.Id)
            .ToList();
    }

    private static IEnumerable<(RecordServiceInfo recordInfo, string attachmentId, bool force)> FilterAndTransformAttachments(
        IEnumerable<Attachment> attachments,
        RecordServiceInfo recordInfo)
    {
        return attachments
            .Where(att => att.UpdatedDate > DateTimeOffset.Now.AddMonths(-DefaultMonthLimit))
            .Take(DefaultLimit)
            .Select(att => (
                recordInfo,
                att.Id,
                false
            ))
            .ToList();
    }

    private static async Task FetchAttachmentContents(
        ServiceHandler serviceHandler,
        IEnumerable<(RecordServiceInfo, string, bool)> filteredAttachments)
    {
        var getAttachmentContentServiceMessage = GetAttachmentContentByRangeService.MakeStartMessage(filteredAttachments);
        await serviceHandler.TryRunServiceAsync(getAttachmentContentServiceMessage);
    }
}
