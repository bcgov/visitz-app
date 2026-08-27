using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.CallDetails;
using VisitzApi.Requests;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.CallDetails;

internal class GetCallInformationService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    List<CallInformationJson> CallInformationData { get; } = [];

    public static string MakeId(EntityType type, string id)
    {
        return $"{nameof(GetCallInformationService)}|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetCallInformationService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, callInformation) = await Vpi.GetCallInformation((ApiRecordType)Info.Type, Info.Id, pagination);
        CallInformationData.AddRange(callInformation);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await CallInformation.SynchronizeAsync(realm, CallInformationData, Info.Id, Info.Type)
        );
    }
}
