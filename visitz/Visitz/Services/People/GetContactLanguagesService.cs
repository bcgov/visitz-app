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
    IcmContact Contact => (IcmContact)Payload;

    List<ContactLanguageJson> contactlanguageRecords { get; } = [];

    public static string MakeId(EntityType type, string parentId, string contactId)
    {
        return $"{nameof(GetContactLanguagesService)}|{type}|{parentId}|{contactId}";
    }

    public static StartServiceMessage MakeStartMessage(IcmContact contact)
    {
        return new()
        {
            ServiceId = MakeId(contact.ParentType, contact.ParentId, contact.Id),
            ServiceType = typeof(GetContactLanguagesService),
            Payload = contact,
        };
    }

    public override string GetId()
    {
        return MakeId(Contact.ParentType, Contact.ParentId, Contact.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, contactlanguages) = await Vpi.GetContactLanguageAsync(
            (ApiRecordType)Contact.ParentType,
            Contact.ParentId,
            Contact.Id,
            pagination
        );
        contactlanguageRecords.AddRange(contactlanguages);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLanguage.SynchronizeAsync(realm, contactlanguageRecords, Contact.Id, Contact.ParentType)
        );
    }
}
