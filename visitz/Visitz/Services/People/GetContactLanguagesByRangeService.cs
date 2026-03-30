using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.People;

#nullable enable
internal class GetContactLanguagesByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<(RecordServiceInfo, string)>(vpi, prefs, serviceHandler)
{
    private IEnumerable<(RecordServiceInfo, string)> ContactLanguageItems =>
        (IEnumerable<(RecordServiceInfo, string)>)Payload;

    public static string MakeId()
    {
        return nameof(GetContactLanguagesByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<(RecordServiceInfo, string)> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetContactLanguagesByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, (RecordServiceInfo, string) item)
    {
        await serviceHandler.TryRunServiceAsync(GetContactLanguagesService.MakeStartMessage(item));
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
