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
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    List<ContactMedicalBehavioralJson> ContactMedicalBehavioralData { get; } = [];

    public static string MakeId(EntityType type, string id)
    {
        return $"{nameof(GetContactMedicalBehavioralService)}|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetContactMedicalBehavioralService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();

        List<(RecordServiceInfo, string)> contactList = [];
        var total = 0;
        var contacts = IcmContact.GetByParentIdAndType(realm, Info.Id, Info.Type);
        foreach (var contact in contacts)
        {
            var contactTuple = (Info, contact.Id);
            contactList.AddRange(contactTuple);

            var (totalCount, contactMedicalBehavioral) = await Vpi.GetContactMedicalBehavioral(
                (ApiRecordType)contact.ParentType,
                contact.ParentId,
                contact.Id,
                pagination
            );
            ContactMedicalBehavioralData.AddRange(contactMedicalBehavioral);
            total += totalCount;
        }

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactMedicalBehavioral.SynchronizeAsync(realm, ContactMedicalBehavioralData, Info.Id, Info.Type)
        );
    }
}
