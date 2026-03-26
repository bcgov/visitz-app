using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.People;

internal class GetContactMedicalBehavioralService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    //RecordServiceInfo Info => (RecordServiceInfo)Payload;
    private (RecordServiceInfo, string) ContactMedicalBehavioralItem => ((RecordServiceInfo, string))Payload;

    List<ContactMedicalBehavioralJson> ContactMedicalBehavioralData { get; } = [];

    public static string MakeId(EntityType type, string id, string contactId)
    {
        return $"{nameof(GetContactMedicalBehavioralService)}|{type}|{id}|{contactId}";
    }

    public static StartServiceMessage MakeStartMessage((RecordServiceInfo info, string contactId) tuple)
    {
        return new()
        {
            ServiceId = MakeId(tuple.info.Type, tuple.info.Id, tuple.contactId),
            ServiceType = typeof(GetContactMedicalBehavioralService),
            Payload = tuple,
        };
    }

    public override string GetId()
    {
        var (info, contactId) = ContactMedicalBehavioralItem;
        return MakeId(info.Type, info.Id, contactId);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (info, contactId) = ContactMedicalBehavioralItem;

        var (total, contactMedicalBehavioral) = await Vpi.GetContactMedicalBehavioral(
            (ApiRecordType)info.Type,
            info.Id,
            contactId,
            pagination
        );
        ContactMedicalBehavioralData.AddRange(contactMedicalBehavioral);

        return total;
    }

    protected override async Task AfterRun()
    {
        var (info, contactId) = ContactMedicalBehavioralItem;

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactMedicalBehavioral.SynchronizeAsync(realm, ContactMedicalBehavioralData, info.Id, info.Type)
        );
    }
}
