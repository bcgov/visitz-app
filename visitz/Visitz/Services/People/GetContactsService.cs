using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

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
        await DownloadAndSaveContacts();

        ResultCode = Result.Successful;
    }

    async Task DownloadAndSaveContacts()
    {
        var contacts = await Vpi.GetContactsAsync((ApiRecordType)Info.Type, Info.Id, after: null);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await IcmContact.SaveContactsAsync(realm, contacts, Info.Id, Info.Type));
    }
}
