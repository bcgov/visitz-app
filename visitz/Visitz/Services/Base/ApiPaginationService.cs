using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Storage;

namespace Visitz.Services.Base;

#nullable enable

internal abstract class ApiPaginationService : VisitzApiService
{
    // TODO: Rearchitect services and start messages with generics to make it
    // easier to pass things like Pagination in
    public Pagination? Pagination { get; set; }

    ParallelOptions ParallelOptions { get; }

    protected ApiPaginationService(
        Vpi vpi,
        LastUpdatedPrefs prefs,
        ParallelOptions? parallelOptions = null) : base(vpi, prefs)
    {
        if (parallelOptions != null
            && (parallelOptions.CancellationToken == default
            || parallelOptions.CancellationToken == CancellationToken.None))
        {
            parallelOptions.CancellationToken = CancelTokenSource.Token;
        }
        
        ParallelOptions = parallelOptions ?? new()
        {
            CancellationToken = CancelTokenSource.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };
    }

    virtual protected Task BeforeRun() { return Task.CompletedTask; }

    abstract protected Task<int> RunPaginatedService(Pagination pagination);

    virtual protected Task AfterRun() { return Task.CompletedTask; }

    protected override sealed async Task RunApiServiceAsync()
    {
        await BeforeRun();
        Pagination ??= new();

        int total = await RunPaginatedService(Pagination);
        if (total > Pagination.PageSize)
        {
            int pages = total / Pagination.PageSize;
            List<Pagination> pagination = [];

            // Start from second page because we already got the first
            for (int page = 1; page <= pages; page++)
                pagination.Add(Pagination.NextPage(page));

            await Parallel.ForEachAsync(
                pagination,
                ParallelOptions,
                async (item, _) => await RunPaginatedService(item));
        }

        await AfterRun();
        ResultCode = Result.Successful;
    }
}
