using Microsoft.Extensions.Logging;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.People;

internal class GetContactsByRangeService(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    ILogger<GetContactsByRangeService> logger)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler, logger)
{
    ServiceHandler ServiceHandler { get; set; } = serviceHandler;

    public static string MakeId()
    {
        return nameof(GetContactsByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> entityIds)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetContactsByRangeService),
            Payload = entityIds,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        await ServiceHandler.TryRunServiceAsync(GetContactsService.MakeStartMessage(item));
    }

    protected override Exception MakePartialException(List<ApiRangeItemException<RecordServiceInfo>> exceptions)
    {
        var outString = exceptions.Select(ex =>
        {
            return $"• {ex.Item.Type} {ex.Item.Label} -> {ex.Message}";
        }).Aggregate((accum, item) => accum + Environment.NewLine + item);

        return new Exception(outString);
    }
}
