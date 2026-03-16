using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.CallDetails;
using VisitzModel.Storage;

namespace Visitz.Services.CallDetails;

#nullable enable

internal class GetIncidentConcernsService(Vpi vpi, LastUpdatedPrefs prefs)
    : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    public static string MakeId(string id)
    {
        return $"{nameof(GetIncidentConcernsService)}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Id),
            ServiceType = typeof(GetIncidentConcernsService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Id);
    }

    override protected async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, incidentConcerns) = await Vpi.GetIncidentConcerns(
            Info.Id,
            pagination);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await IncidentConcerns.SynchronizeAsync(realm, incidentConcerns));

        return total;
    }
}
