using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;

namespace Visitz.Services.Visits;

#nullable enable

internal class PostAndRefreshVisitService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiService(vpi, prefs)
{
    ServiceHandler ServiceHandler => serviceHandler;

    PersonVisit Visit => (PersonVisit)Payload;

    public static string MakeId(string caseId)
    {
        return $"{nameof(PostAndRefreshVisitService)}|{caseId}";
    }

    public static string MakeId(PersonVisit visit)
    {
        return MakeId(visit.ParentId);
    }

    public static StartServiceMessage MakeStartMessage(PersonVisit visitToSend)
    {
        return new()
        {
            Payload = visitToSend,
            ServiceId = MakeId(visitToSend),
            ServiceType = typeof(PostAndRefreshVisitService),
        };
    }

    public override string GetId()
    {
        return MakeId(Visit);
    }

    protected override async Task RunApiServiceAsync()
    {
        if (await PostVisit() && await RefreshVisit())
            ResultCode = Result.Successful;
    }

    async Task<bool> PostVisit()
    {
        var result = await ServiceHandler.TryRunServiceAsync(PostVisitService.MakeStartMessage(Visit));
        return result == Result.Successful;
    }

    async Task<bool> RefreshVisit()
    {
        var result = await ServiceHandler.TryRunServiceAsync(GetVisitsService.MakeStartMessage(Visit.ParentId));
        return result == Result.Successful;
    }
}
