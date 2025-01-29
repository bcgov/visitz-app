using Microsoft.Extensions.Logging;
using VisitzApi;
using VisitzModel.Storage;

namespace Visitz.Services.Base;

internal abstract class VisitzApiRangeService<Item>(
    Vpi vpi,
    LastUpdatedPrefs prefs,
    ServiceHandler serviceHandler,
    ILogger logger)
    : VisitzApiService(vpi, prefs)
{
    ServiceHandler ServiceHandler => serviceHandler;

    IEnumerable<Item> Items => (IEnumerable<Item>)Payload;

    List<ApiRangeItemException<Item>> Exceptions { get; } = [];

    ILogger Logger { get; } = logger;

    protected override sealed async Task RunApiServiceAsync()
    {
        await Parallel.ForEachAsync(Items, RunForItemParallelAsync);

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
