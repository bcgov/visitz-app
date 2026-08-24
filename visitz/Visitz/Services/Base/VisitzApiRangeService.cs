using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Base;

internal abstract class VisitzApiRangeService<Item>(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    CancellationToken? cancellationToken = null,
    int? maxDegreeOfParallelism = null
) : VisitzApiService(vpi, prefs)
{
    protected ServiceHandler ServiceHandler => serviceHandler;

    IEnumerable<Item> Items => (IEnumerable<Item>)Payload;

    public ParallelOptions ParalellOptions { get; } =
        new()
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism ?? ParallelServiceDefaults.MaxParallelism,
            CancellationToken = cancellationToken ?? CancellationToken.None,
        };

    List<ApiRangeItemException<Item>> Exceptions { get; } = [];

    protected sealed override async Task RunApiServiceAsync()
    {
        await Parallel.ForEachAsync(Items, ParalellOptions, RunForItemParallelAsync);

        ResultCode = Exceptions.Count <= 0 ? Result.Successful : throw MakeOverallException(Exceptions);
    }

    async ValueTask RunForItemParallelAsync(Item item, CancellationToken token)
    {
        try
        {
            await RunInParallelAsync(ServiceHandler, item);
        }
        catch (Exception ex)
        {
            Exceptions.Add(new ApiRangeItemException<Item>(item, ex));
            Logger.LogException(ex);
        }
    }

    protected abstract Task RunInParallelAsync(ServiceHandler serviceHandler, Item item);

    static Exception MakeOverallException(List<ApiRangeItemException<Item>> exceptions)
    {
        return exceptions.Count == 1 ? exceptions[0] : new AggregateException(exceptions);
    }
}
