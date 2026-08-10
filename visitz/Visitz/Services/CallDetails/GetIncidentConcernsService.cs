using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.CallDetails;
using VisitzApi.Requests;
using VisitzModel.Models.CallDetails;
using VisitzModel.Storage;

namespace Visitz.Services.CallDetails;

internal class GetIncidentConcernsService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    List<IncidentConcernsJson> IncidentConcernRecords { get; } = [];

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

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, incidentConcerns) = await Vpi.GetIncidentConcerns(Info.Id, pagination);

        IncidentConcernRecords.AddRange(incidentConcerns);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await IncidentConcerns.SynchronizeAsync(realm, IncidentConcernRecords, Info.Id)
        );
    }
}
