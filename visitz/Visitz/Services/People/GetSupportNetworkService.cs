using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

internal class GetSupportNetworkService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    readonly ConcurrentBag<SupportNetworkJson> _supportNetworks = [];

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

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, supportNetworks) = await Vpi.GetSupportNetworkAsync((ApiRecordType)Info.Type, Info.Id, pagination);

        _supportNetworks.AddAll(supportNetworks);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await SupportNetworkItem.SynchronizeAsync(realm, _supportNetworks, Info.Id, Info.Type)
        );
    }
}
