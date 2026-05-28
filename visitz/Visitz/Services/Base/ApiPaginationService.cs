using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Storage;

namespace Visitz.Services.Base;

#nullable enable

internal abstract class ApiPaginationService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    // TODO: Rearchitect services and start messages with generics to make it
    // easier to pass things like Pagination in
    public Pagination? Pagination { get; set; }

    public ParallelOptions ParallelOptions { get; } =
        new()
        {
            CancellationToken = CancellationToken.None,
            MaxDegreeOfParallelism = ParallelServiceDefaults.MaxParallelism,
        };

    protected List<Exception> Exceptions { get; } = [];

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
