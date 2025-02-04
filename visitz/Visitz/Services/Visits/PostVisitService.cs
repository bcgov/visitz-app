using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Storage;

namespace Visitz.Services.Visits;

internal class PostVisitService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    PersonVisit Visit => (PersonVisit)Payload;

    public static string MakeId(PersonVisit visit)
    {
        return $"{nameof(PostVisitService)}|{visit.ParentId}";
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
        await PostVisitAsync();

        ResultCode = Result.Successful;
    }

    async Task PostVisitAsync()
    {
        await Vpi.PostVisitAsync(Visit.ParentId, Visit.ToApiJson());
    }
}
