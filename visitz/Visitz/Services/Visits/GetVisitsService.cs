using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;

namespace Visitz.Services.Visits;

#nullable enable

internal class GetVisitsService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    readonly ConcurrentBag<VisitJson> _visits = [];

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

    protected override async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, visits) = await Vpi.GetVisitsAsync(CaseId, pagination);

        _visits.AddAll(visits);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await PersonVisit.SynchronizeAsync(realm, _visits, CaseId)
        );
    }
}
