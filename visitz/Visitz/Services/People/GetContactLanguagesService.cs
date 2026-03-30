using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

#nullable enable
internal class GetContactLanguagesService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    // RecordServiceInfo Info => (RecordServiceInfo)Payload;
    private (RecordServiceInfo, string) Info => ((RecordServiceInfo, string))Payload;

    List<ContactLanguageJson> contactlanguageRecords { get; } = [];

    public static string MakeId(EntityType type, string id, string contactId)
    {
        return $"{nameof(GetContactLanguagesService)}|{type}|{id}|{contactId}";
    }

    public static StartServiceMessage MakeStartMessage((RecordServiceInfo recordServiceInfo, string contactId) tuple)
    {
        return new()
        {
            ServiceId = MakeId(tuple.recordServiceInfo.Type, tuple.recordServiceInfo.Id, tuple.contactId),
            ServiceType = typeof(GetContactLanguagesService),
            Payload = tuple,
        };
    }

    public override string GetId()
    {
        var (recordServiceInfo, contactId) = Info;
        return MakeId(recordServiceInfo.Type, recordServiceInfo.Id, contactId);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (recordServiceInfo, contactId) = Info;
        var (total, contactlanguages) = await Vpi.GetContactLanguageAsync(
            (ApiRecordType)recordServiceInfo.Type,
            recordServiceInfo.Id,
            contactId,
            pagination
        );
        contactlanguageRecords.AddRange(contactlanguages);
        return total;
    }

    protected override async Task AfterRun()
    {
        var (info, contactId) = Info;
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLanguage.SynchronizeAsync(realm, contactlanguageRecords, info.Id, info.Type)
        );
    }
}
