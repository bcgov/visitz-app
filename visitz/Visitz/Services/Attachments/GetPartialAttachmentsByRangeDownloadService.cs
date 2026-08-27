using Realms;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage;

namespace Visitz.Services.Attachments;

internal class GetPartialAttachmentsByRangeDownloadService(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    UserIgnoredContentPrefs userIgnoredContentPrefs
) : VisitzApiService(vpi, prefs)
{
    static readonly int DefaultLimit = 10;
    static readonly int DefaultMonthLimit = 3;
    ServiceHandler ServiceHandler { get; set; } = serviceHandler;
    UserIgnoredContentPrefs UserIgnoredPrefs { get; } = userIgnoredContentPrefs;

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
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    IEnumerable<RecordServiceInfo> Items => (IEnumerable<RecordServiceInfo>)Payload;

    protected override async Task RunApiServiceAsync()
    {
        IEnumerable<(RecordServiceInfo, string, bool force)> allFilteredAttachments = [];

        foreach (var item in Items)
        {
            var filteredAttachments = await ProcessAttachmentsAsync(item);
            allFilteredAttachments = allFilteredAttachments.Concat(filteredAttachments);
        }

        if (allFilteredAttachments.Any())
        {
            await FetchAttachmentContents(allFilteredAttachments);
            ResultCode = Result.Successful;
        }
        else
            ResultCode = Result.NoOperation;
    }

    private async Task<
        IEnumerable<(RecordServiceInfo recordInfo, string attachmentId, bool force)>
    > ProcessAttachmentsAsync(RecordServiceInfo recordInfo)
    {
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();

        var allAttachments = Attachment.GetOrderedAttachments(realm, recordInfo.Type, recordInfo.Id).Freeze();
        var filteredAttachments = FilterAndTransformAttachments(allAttachments, recordInfo);

        return filteredAttachments;
    }

    private IEnumerable<(RecordServiceInfo recordInfo, string attachmentId, bool force)> FilterAndTransformAttachments(
        IQueryable<Attachment> attachments,
        RecordServiceInfo recordInfo
    )
    {
        var monthThreshold = DateTimeOffset.Now.AddMonths(-DefaultMonthLimit);

        var limitedAttachments = attachments
            .Where(att => att.UpdatedDate > monthThreshold)
            .AsEnumerable()
            .Take(DefaultLimit);
        return limitedAttachments
            .Where(att => UserIgnoredPrefs?.GetUserIgnoredContent(att.Id) != true)
            .Select(att => (recordInfo, att.Id, false));
    }

    private async Task FetchAttachmentContents(IEnumerable<(RecordServiceInfo, string, bool force)> filteredAttachments)
    {
        var startMessage = GetAttachmentContentByRangeService.MakeStartMessage(filteredAttachments);
        await ServiceHandler.TryRunServiceAsync(startMessage);
    }
}
