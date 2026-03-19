using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Visits;

internal class GetVisitsByRangeService(Vpi vpi, ServiceHandler serviceHandler, LastUpdatedPrefs prefs)
    : VisitzApiService(vpi, prefs)
{
    public static string MakeId(IEnumerable<string> caseIds)
    {
        return nameof(GetVisitsByRangeService) + caseIds.GetHashCode();
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<string> caseIds)
    {
        return new()
        {
            ServiceId = MakeId(caseIds),
            ServiceType = typeof(GetVisitsByRangeService),
            Payload = caseIds,
        };
    }

    ServiceHandler ServiceHandler => serviceHandler;

    IEnumerable<string> CaseIds => (IEnumerable<string>)Payload;

    readonly List<string> successIds = [];

    readonly List<string> erroredIds = [];

    public override string GetId()
    {
        return MakeId(CaseIds);
    }

    protected override async Task RunApiServiceAsync()
    {
        await Parallel.ForEachAsync(CaseIds, GetVisitsForCase);

        ResultCode =
            erroredIds.Count <= 0
                ? Result.Successful
                : throw new PartialRangeErrorException(nameof(GetVisitsByRangeService), successIds, erroredIds);
    }

    async ValueTask GetVisitsForCase(string caseId, CancellationToken token)
    {
        try
        {
            await ServiceHandler.TryRunServiceAsync(GetVisitsService.MakeStartMessage(caseId));
            successIds.Add(caseId);
        }
        catch (Exception ex)
        {
            erroredIds.Add($"{caseId} -> {ex.Message}");
        }
    }
}
