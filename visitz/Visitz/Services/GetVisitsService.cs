using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzModel.Models;

namespace Visitz.Services;

internal class GetVisitsService(Vpi vpi) : VisitzApiService(vpi)
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
        await GetVisitsAsync();

        ResultCode = Result.Successful;
    }

    async Task GetVisitsAsync()
    {
        var visits = await Vpi.GetVisitsAsync(CaseId, after: null);
        using var realm = await VisitzRealms.GetIcmDataRealmAsync();

        await PersonVisit.SaveVisitsAsync(realm, visits);
    }
}
