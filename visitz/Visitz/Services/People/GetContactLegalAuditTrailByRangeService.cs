using System;
using System.Collections.Generic;
using System.Text;
using Visitz.Services.Base;
using Visitz.Services.CallDetails;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;
#nullable enable
namespace Visitz.Services.People;

internal class GetContactLegalAuditTrailByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<(RecordServiceInfo, string)>(vpi, prefs, serviceHandler)
{
    private IEnumerable<(RecordServiceInfo, string)> ContactlanguageaudittrailItems =>
        (IEnumerable<(RecordServiceInfo, string)>)Payload;

    public static string MakeId()
    {
        return nameof(GetContactLegalAuditTrailByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<(RecordServiceInfo, string)> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetContactLegalAuditTrailByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, (RecordServiceInfo, string) tuple)
    {
        await serviceHandler.TryRunServiceAsync(GetContactLegalAuditTrailService.MakeStartMessage(tuple));
    }

    protected override Exception MakePartialException(
        List<ApiRangeItemException<(RecordServiceInfo, string)>> exceptions
    )
    {
        var recordServiceInfoExceptions = exceptions
            .Select(ex => new ApiRangeItemException<RecordServiceInfo>(ex.Item.Item1, ex.InnerException))
            .ToList();

        return recordServiceInfoExceptions.CombineIntoException();
    }
}
