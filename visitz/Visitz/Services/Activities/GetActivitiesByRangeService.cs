using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Activities;

internal class GetActivitiesByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler, maxDegreeOfParallelism: 1)
{
    public static string MakeId()
    {
        return nameof(GetActivitiesByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetActivitiesByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        await serviceHandler.TryRunServiceAsync(GetActivitiesService.MakeStartMessage(item));
    }
}
