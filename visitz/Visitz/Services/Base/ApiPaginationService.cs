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

    protected List<Exception> Exceptions { get; } = [];

    protected ApiPaginationService(Vpi vpi, LastUpdatedPrefs prefs, ParallelOptions? parallelOptions = null)
        : base(vpi, prefs)
    {
        if (
            parallelOptions != null
            && (
                parallelOptions.CancellationToken == default
                || parallelOptions.CancellationToken == CancellationToken.None
            )
        )
        {
            parallelOptions.CancellationToken = CancelTokenSource.Token;
        }

        ParallelOptions =
            parallelOptions
            ?? new()
            {
                CancellationToken = CancelTokenSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };
    }

    protected virtual Task BeforeRun()
    {
        return Task.CompletedTask;
    }

    async Task<int> TryRunPaginatedService(Pagination pagination)
    {
        try
        {
            return await RunPageInParallelAsync(pagination);
        }
        catch (Exception ex)
        {
            Exceptions.Add(ex);
            return int.MinValue;
        }
    }

    protected abstract Task<int> RunPageInParallelAsync(Pagination pagination);

    protected virtual Task AfterRun()
    {
        return Task.CompletedTask;
    }

    protected sealed override async Task RunApiServiceAsync()
    {
        await BeforeRun();
        Pagination ??= new();

        int total = await TryRunPaginatedService(Pagination);
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
                async (item, _) => await TryRunPaginatedService(item)
            );
        }

        try
        {
            await AfterRun();
        }
        catch (Exception ex)
        {
            Exceptions.Add(ex);
        }

        if (Exceptions.Count > 1)
            throw new AggregateException(Exceptions);
        else if (Exceptions.Count > 0)
            throw Exceptions.First();

        ResultCode = Result.Successful;
    }
}
