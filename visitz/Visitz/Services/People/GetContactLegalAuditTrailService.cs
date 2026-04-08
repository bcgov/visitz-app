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

internal class GetContactLegalAuditTrailService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    //private (RecordServiceInfo, string) ContactAuditTrailItem => ((RecordServiceInfo, string))Payload;

    RecordServiceInfo Info => (RecordServiceInfo)Payload;
    List<ContactLegalAuditTrailJson> AuditTrailData { get; } = [];

    public static string MakeId(EntityType type, string id) //, string contactId)
    {
        return $"{nameof(GetContactLegalAuditTrailService)}|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetContactLegalAuditTrailService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        //var (info, contactId) = ContactAuditTrailItem;
        //var (total, contactIds) = await Vpi.GetContactLegalAuditTrail(
        //    (ApiRecordType)info.Type,
        //    info.Id,
        //    contactId,
        //    pagination
        //);
        //AuditTrailData.AddRange(contactIds);

        //return total;

        using var realm = await VisitzRealms.GetIcmDataRealmAsync();

        List<(RecordServiceInfo, string)> contactList = [];
        var total = 0;
        var contacts = IcmContact.GetByParentIdAndType(realm, Info.Id, Info.Type);
        foreach (var contact in contacts)
        {
            var contactTuple = (Info, contact.Id);
            contactList.AddRange(contactTuple);

            var (totalCount, contactLegalAuditTrail) = await Vpi.GetContactLegalAuditTrail(
                (ApiRecordType)contact.ParentType,
                contact.ParentId,
                contact.Id,
                pagination
            );
            AuditTrailData.AddRange(contactLegalAuditTrail);
            total += totalCount;
        }

        return total;
    }

    protected override async Task AfterRun()
    {
        //var (info, contactId) = ContactAuditTrailItem;

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLegalAuditTrail.SynchronizeAsync(realm, AuditTrailData, Info.Id, Info.Type)
        );
    }
}
