using System.Collections.Concurrent;
using Visitz.Services.Base;
using Visitz.Services.Messages;
using Visitz.Storage;
using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Extensions;
using VisitzModel.Models.ListOfValues;
using VisitzModel.Storage;

namespace Visitz.Services.ListOfValues;

internal class GetLookupValuesService(Vpi vpi, LastUpdatedPrefs prefs) : ApiPaginationService(vpi, prefs)
{
    string LookupKeyName => (string)Payload;

    readonly ConcurrentBag<ListedValue> _listedValues = [];

    public static string MakeId(string lookupKeyName)
    {
        return $"{nameof(GetLookupValuesService)}|{lookupKeyName}";
    }

    public static StartServiceMessage MakeStartMessage(string lookupKeyName)
    {
        return new()
        {
            ServiceId = MakeId(lookupKeyName),
            ServiceType = typeof(GetLookupValuesService),
            Payload = lookupKeyName,
        };
    }

    public override string GetId()
    {
        return MakeId(LookupKeyName);
    }

    protected override async Task<int> RunPageInParallelAsync(Pagination pagination)
    {
        var (count, items) = await Vpi.GetListOfValuesAsync(LookupKeyName, pagination);

        _listedValues.AddAll(items.Select(item => new ListedValue(item)));

        return count;
    }

    protected override async Task AfterRun()
    {
        await base.AfterRun();

        await VisitzRealms.EnqueueIcmDataActionAsync(async realm =>
            await ListedValue.SynchronizeAsync(realm, LookupKeyName, _listedValues)
        );
    }
}
