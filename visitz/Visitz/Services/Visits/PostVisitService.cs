using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Views.Debugging;
using VisitzApi;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;

namespace Visitz.Services.Visits;

internal class PostVisitService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    PersonVisit Visit => (PersonVisit)Payload;

    public static string MakeId(string caseId)
    {
        return $"{nameof(PostVisitService)}|{caseId}";
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
            ServiceType = typeof(PostVisitService),
        };
    }

    public override string GetId()
    {
        return MakeId(Visit);
    }

    protected override async Task RunApiServiceAsync()
    {
        if (DebugOptions.Default.DryFirePostVisitService)
        {
            await Task.Delay(2000); // Simulate network activity
            ResultCode = DebugOptions.Default.DryFirePostVisitServiceSimulateSuccess ? Result.Successful : Result.Error;
        }
        else
            await PostVisitAsync();

        ResultCode = Result.Successful;
    }

    async Task PostVisitAsync()
    {
        await Vpi.PostVisitAsync(Visit.ParentId, Visit.ToApiJson("yyyy-MM-dd"));
    }
}
