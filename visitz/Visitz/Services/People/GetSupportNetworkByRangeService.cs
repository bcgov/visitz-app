using Microsoft.Extensions.Logging;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.People;

internal class GetSupportNetworkByRangeService(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    ILogger logger)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler, logger)
{

    public static string MakeId()
    {
        return nameof(GetSupportNetworkByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetSupportNetworkByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        await serviceHandler.TryRunServiceAsync(GetSupportNetworkService.MakeStartMessage(item));
    }

    protected override Exception MakePartialException(List<ApiRangeItemException<RecordServiceInfo>> exceptions)
    {
        return exceptions.CombineIntoException();
    }
}
