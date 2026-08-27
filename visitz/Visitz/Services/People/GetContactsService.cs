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

internal class GetContactsService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    RecordServiceInfo Info => (RecordServiceInfo)Payload;

    readonly ConcurrentBag<ContactJson> _contacts = [];

    public static string MakeId(EntityType type, string id)
    {
        return nameof(GetContactsService) + $"|{type}|{id}";
    }

    public static StartServiceMessage MakeStartMessage(RecordServiceInfo info)
    {
        return new()
        {
            ServiceId = MakeId(info.Type, info.Id),
            ServiceType = typeof(GetContactsService),
            Payload = info,
        };
    }

    public override string GetId()
    {
        return MakeId(Info.Type, Info.Id);
    }

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (total, contacts) = await Vpi.GetContactsAsync((ApiRecordType)Info.Type, Info.Id, pagination);

        _contacts.AddAll(contacts);

        return total;
    }

    protected override async Task AfterRun()
    {
        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await IcmContact.SynchronizeAsync(realm, _contacts, Info.Id, Info.Type)
        );
    }
}
