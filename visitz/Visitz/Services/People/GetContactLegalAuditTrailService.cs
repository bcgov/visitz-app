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
    private (RecordServiceInfo, string) ContactAuditTrailItem => ((RecordServiceInfo, string))Payload;

    List<ContactLegalAuditTrailJson> AuditTrailData { get; } = [];

    public static string MakeId(EntityType type, string id, string contactId)
    {
        return $"{nameof(GetContactLegalAuditTrailService)}|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage((RecordServiceInfo info, string contactId) tuple)
    {
        return new()
        {
            ServiceId = MakeId(tuple.info.Type, tuple.info.Id, tuple.contactId),
            ServiceType = typeof(GetContactLegalAuditTrailService),
            Payload = tuple,
        };
    }

    public override string GetId()
    {
        var (info, contactId) = ContactAuditTrailItem;
        return MakeId(info.Type, info.Id, contactId);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (info, contactId) = ContactAuditTrailItem;
        var (total, contactIds) = await Vpi.GetContactLegalAuditTrail(
            (ApiRecordType)info.Type,
            info.Id,
            contactId,
            pagination
        );
        AuditTrailData.AddRange(contactIds);

        return total;
    }

    protected override async Task AfterRun()
    {
        var (info, contactId) = ContactAuditTrailItem;

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLegalAuditTrail.SynchronizeAsync(realm, AuditTrailData, info.Id, info.Type)
        );
    }
}
