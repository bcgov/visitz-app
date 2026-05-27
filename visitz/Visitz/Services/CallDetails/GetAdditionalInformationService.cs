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

#nullable enable
internal class GetAdditionalInformationService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;
    List<AdditionalInformationJson> AdditionalInformationRecords { get; } = [];

    public static string MakeId(EntityType type, string id)
    {
        return nameof(GetAdditionalInformationService) + $"|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetAdditionalInformationService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, contacts) = await Vpi.GetAdditionalInformation((ApiRecordType)Info.Type, Info.Id, pagination);
        AdditionalInformationRecords.AddRange(contacts);
        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await AdditionalInformation.SynchronizeAsync(realm, AdditionalInformationRecords, Info.Id, Info.Type)
        );
    }
}
