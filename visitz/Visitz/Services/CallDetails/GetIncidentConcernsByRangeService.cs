using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.CallDetails;

internal class GetIncidentConcernsByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler)
{
    public static string MakeId()
    {
        return nameof(GetIncidentConcernsByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetIncidentConcernsByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        await serviceHandler.TryRunServiceAsync(GetIncidentConcernsService.MakeStartMessage(item));
    }
}
