using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Notes;

#nullable enable

internal class GetNotesForRangeService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
    : VisitzApiRangeService<RecordServiceInfo>(vpi, prefs, serviceHandler)
{
    public static string MakeId()
    {
        return nameof(GetNotesForRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<RecordServiceInfo> idEntityItems)
    {
        return new StartServiceMessage()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetNotesForRangeService),
            Payload = idEntityItems,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, RecordServiceInfo item)
    {
        await ServiceHandler.TryRunServiceAsync(GetNotesService.MakeStartMessage(item));
    }
}
