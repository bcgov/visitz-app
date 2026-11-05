using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Models.Activities;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.Activities;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;

namespace Visitz.Services.Activities;

internal class GetActivitiesService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;
    readonly ConcurrentBag<ActivityJson> _activities = [];

    public static string MakeId(EntityType type, string id)
    {
        return $"{nameof(GetActivitiesService)}|{type}|{id}";
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetActivitiesService),
            Payload = info,
        };
    }

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, activities) = await Vpi.GetActivitiesAsync((ApiRecordType)Info.Type, Info.Id, pagination);
        _activities.AddAll(activities);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await Activity.SynchronizeAsync(realm, _activities, Info.Id, Info.Type)
        );
    }
}
