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
        IEnumerable<(RecordServiceInfo, string, bool)> allFilteredAttachments = [];

        foreach (var item in Items)
        {
            var filteredAttachments = await ProcessAttachmentsAsync(serviceHandler, item);
            allFilteredAttachments = allFilteredAttachments.Concat(filteredAttachments);
        }

        if (allFilteredAttachments.Any())
        {
            await FetchAttachmentContents(serviceHandler, allFilteredAttachments);
        }
    }

    private static async Task<IEnumerable<
        (RecordServiceInfo recordInfo,
        string attachmentId,
        bool force)>> ProcessAttachmentsAsync(ServiceHandler serviceHandler, RecordServiceInfo recordInfo)
    {
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();

        var allAttachments = Attachment.GetAttachments(realm, recordInfo.Type, recordInfo.Id).Freeze();
        var filteredAttachments = FilterAndTransformAttachments(allAttachments, recordInfo);

        return filteredAttachments;
    }

    private static IEnumerable<
        (RecordServiceInfo recordInfo,
        string attachmentId,
        bool force)> FilterAndTransformAttachments(IQueryable<Attachment> attachments, RecordServiceInfo recordInfo)
    {
        var monthThreshold = DateTimeOffset.Now.AddMonths(-DefaultMonthLimit);

        return attachments
            .Where(att => att.UpdatedDate > monthThreshold)
            .AsEnumerable()
            .Take(DefaultLimit)
            .Select(att => (
                recordInfo,
                att.Id,
                false
            ));
    }

    private static async Task FetchAttachmentContents(
        ServiceHandler serviceHandler,
        IEnumerable<(RecordServiceInfo, string, bool)> filteredAttachments)
    {
        var startMessage = GetAttachmentContentByRangeService.MakeStartMessage(filteredAttachments);
        await serviceHandler.TryRunServiceAsync(startMessage);
    }
}
