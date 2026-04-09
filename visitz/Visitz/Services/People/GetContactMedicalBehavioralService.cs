using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.People;

internal class GetContactMedicalBehavioralService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    IcmContact Contact => (IcmContact)Payload;

    List<ContactMedicalBehavioralJson> ContactMedicalBehavioralData { get; } = [];

    public static string MakeId(string parentContactId)
    {
        return $"{nameof(GetContactMedicalBehavioralService)}|{parentContactId}";
    }

    public static StartServiceMessage MakeStartMessage(IcmContact contact)
    {
        return new()
        {
            ServiceId = MakeId(contact.Id),
            ServiceType = typeof(GetContactMedicalBehavioralService),
            Payload = contact,
        };
    }

    public override string GetId()
    {
        return MakeId(Contact.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (totalCount, contactMedicalBehavioral) = await Vpi.GetContactMedicalBehavioral(
            (ApiRecordType)Contact.ParentType,
            Contact.ParentId,
            Contact.Id,
            pagination
        );

        ContactMedicalBehavioralData.AddRange(contactMedicalBehavioral);

        return totalCount;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactMedicalBehavioral.SynchronizeAsync(realm, ContactMedicalBehavioralData, Contact.Id)
        );
    }
}
