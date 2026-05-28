using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.People;

internal class GetContactMedicalBehavioralByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<IcmContact>(vpi, prefs, serviceHandler)
{
    public static string MakeId()
    {
        return nameof(GetContactMedicalBehavioralByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<IcmContact> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetContactMedicalBehavioralByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, IcmContact item)
    {
        await serviceHandler.TryRunServiceAsync(GetContactMedicalBehavioralService.MakeStartMessage(item));
    }

    protected override Exception MakeOverallException(List<ApiRangeItemException<IcmContact>> exceptions)
    {
        var outString = exceptions
            .Select(ex =>
            {
                return $"• {ex.Item.ParentType} {ex.Item.ParentId} {ex.Item.Id} -> {ex.Message}";
            })
            .Aggregate((accum, item) => accum + Environment.NewLine + item);

        return new Exception(outString);
    }
}
