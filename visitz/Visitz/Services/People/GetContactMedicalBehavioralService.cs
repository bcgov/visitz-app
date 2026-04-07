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
    IcmContact Contact => (IcmContact)Payload;

    List<ContactMedicalBehavioralJson> ContactMedicalBehavioralData { get; } = [];

    public static string MakeId(EntityType type, string parentId, string id)
    {
        return $"{nameof(GetContactMedicalBehavioralService)}|{type}|{parentId}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(IcmContact contact)
    {
        return new()
        {
            ServiceId = MakeId(contact.ParentType, contact.ParentId, contact.Id),
            ServiceType = typeof(GetContactMedicalBehavioralService),
            Payload = contact,
        };
    }

    public override string GetId()
    {
        return MakeId(Contact.ParentType, Contact.ParentId, Contact.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (totalCount, contactMedicalBehavioral) = await Vpi.GetContactMedicalBehavioral(
            (ApiRecordType)Contact.ParentType,
            Contact.ParentId,
            Contact.Id,
            pagination
        );

        return totalCount;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactMedicalBehavioral.SynchronizeAsync(
                realm,
                ContactMedicalBehavioralData,
                Contact.Id,
                Contact.ParentType
            )
        );
    }
}
