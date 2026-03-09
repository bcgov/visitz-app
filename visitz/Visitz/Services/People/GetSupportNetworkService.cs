using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

#nullable enable

internal class GetSupportNetworkService(Vpi vpi, LastUpdatedPrefs prefs)
    : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    public static string MakeId(EntityType type, string id)
    {
        return $"{nameof(GetSupportNetworkService)}|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetSupportNetworkService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    override protected async Task<int> RunPaginatedService(Pagination pagination)
    {
        var (total, supportNetwork) = await Vpi.GetSupportNetworkAsync(
            (ApiRecordType)Info.Type,
            Info.Id,
            pagination);

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await SupportNetworkItem.SynchronizeAsync(realm, supportNetwork, Info.Id, Info.Type));

        return total;
    }
}
