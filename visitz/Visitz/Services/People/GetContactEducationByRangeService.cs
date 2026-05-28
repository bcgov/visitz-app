using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.People;

internal class GetContactEducationByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<IcmContact>(vpi, prefs, serviceHandler)
{
    public static string MakeId()
    {
        return nameof(GetContactEducationByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<IcmContact> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetContactEducationByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, IcmContact item)
    {
        await serviceHandler.TryRunServiceAsync(GetContactEducationService.MakeStartMessage(item));
    }
}
