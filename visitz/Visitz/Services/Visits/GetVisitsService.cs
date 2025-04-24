using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;

namespace Visitz.Services.Visits;

#nullable enable

internal class GetVisitsService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    public static string MakeId(string caseId)
    {
        return nameof(GetVisitsService) + caseId;
    }

    public static StartServiceMessage MakeStartMessage(string caseId)
    {
        return new()
        {
            ServiceId = MakeId(caseId),
            ServiceType = typeof(GetVisitsService),
            Payload = caseId,
        };
    }

    string CaseId => (string)Payload;

    public override string GetId()
    {
        return MakeId(CaseId);
    }

    protected override async Task RunApiServiceAsync()
    {
        Pagination pagination = new();
        int total = await GetVisitsAsync(pagination);

        if (total > pagination.PageSize)
            await Task.WhenAll(UnrollPagination(
                total,
                pagination.PageSize,
                GetVisitsAsync));

        ResultCode = Result.Successful;
    }

    async Task<int> GetVisitsAsync(Pagination? pagination = null)
    {
        var (total, visits) = await Vpi.GetVisitsAsync(CaseId, pagination);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await PersonVisit.SaveVisitsAsync(realm, visits));

        return total;
    }
}
