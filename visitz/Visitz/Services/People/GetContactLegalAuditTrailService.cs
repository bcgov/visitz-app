using System;
using System.Collections.Generic;
using System.Text;
using Visitz.Services.Base;
using Visitz.Services.CallDetails;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.CallDetails;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;
#nullable enable
namespace Visitz.Services.People;

internal class GetContactLegalAuditTrailService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    private (RecordServiceInfo, string) ContactaudittrailItem => ((RecordServiceInfo, string))Payload;

    List<ContactLegalAuditTrailJson> AudittrailData { get; } = [];

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
        var (info, contactId) = ContactaudittrailItem;
        return MakeId(info.Type, info.Id, contactId);
    }

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (info, contactId) = ContactaudittrailItem;
        var (total, contactIds) = await Vpi.GetContactLegalaudittrail(
            (ApiRecordType)info.Type,
            info.Id,
            contactId,
            pagination
        );
        AudittrailData.AddRange(contactIds);

        return total;
    }

    protected override async Task AfterRun()
    {
        var (info, contactId) = ContactaudittrailItem;

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ContactLegalAuditTrail.SynchronizeAsync(realm, AudittrailData, info.Id, info.Type)
        );
    }
}
