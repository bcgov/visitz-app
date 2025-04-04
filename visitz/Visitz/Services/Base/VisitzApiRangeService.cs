using Microsoft.Extensions.Logging;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Base;

internal abstract class VisitzApiRangeService<Item>(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    ParallelOptions options = null)
    : VisitzApiService(vpi, prefs)
{
    protected ServiceHandler ServiceHandler => serviceHandler;

    IEnumerable<Item> Items => (IEnumerable<Item>)Payload;

    List<ApiRangeItemException<Item>> Exceptions { get; } = [];

    protected override sealed async Task RunApiServiceAsync()
    {
        if (options == null)
            await Parallel.ForEachAsync(Items, RunForItemParallelAsync);
        else
            await Parallel.ForEachAsync(Items, options, RunForItemParallelAsync);

        ResultCode = Exceptions.Count <= 0
            ? Result.Successful
            : throw MakePartialException(Exceptions);
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
            Logger.LogError(ex, ex.ToString());
        }
    }

    protected abstract Task RunInParallelAsync(ServiceHandler serviceHandler, Item item);

    protected abstract Exception MakePartialException(List<ApiRangeItemException<Item>> exceptions);
}
