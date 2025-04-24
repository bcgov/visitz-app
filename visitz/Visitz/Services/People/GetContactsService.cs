using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

#nullable enable

internal class GetContactsService(Vpi vpi, LastUpdatedPrefs prefs)
    : VisitzApiService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    public static string MakeId(EntityType type, string id)
    {
        return nameof(GetContactsService) + $"|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetContactsService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task RunApiServiceAsync()
    {
        Pagination pagination = new();
        int total = await DownloadAndSaveContacts(pagination);

        if (total > pagination.PageSize)
            await Task.WhenAll(UnrollPagination(
                total,
                pagination.PageSize,
                DownloadAndSaveContacts));

        ResultCode = Result.Successful;
    }

    async Task<int> DownloadAndSaveContacts(Pagination? pagination = null)
    {
        var (total, contacts) = await Vpi.GetContactsAsync(
            (ApiRecordType)Info.Type,
            Info.Id,
            pagination);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await IcmContact.SaveContactsAsync(realm, contacts, Info.Id, Info.Type));

        return total;
    }
}
