using VisitzApi;
using VisitzApi.Requests;
using VisitzModel.Storage;

namespace Visitz.Services.Base;

#nullable enable

internal abstract class ApiPaginationService(Vpi vpi, LastUpdatedPrefs prefs)
    : VisitzApiService(vpi, prefs)
{
    // TODO: Rearchitect services and start messages with generics to make it
    // easier to pass things like Pagination in
    public Pagination? Pagination { get; set; }

    virtual protected Task BeforeRun() { return Task.CompletedTask; }

    abstract protected Task<int> RunPaginatedService(Pagination pagination);

    virtual protected Task AfterRun() { return Task.CompletedTask; }

    protected override sealed async Task RunApiServiceAsync()
    {
        await BeforeRun();
        Pagination ??= new();

        int total = await RunPaginatedService(Pagination);

        if (total > Pagination.PageSize)
            await Task.WhenAll(UnrollPagination(
                total,
                Pagination.PageSize,
                RunPaginatedService));

        await AfterRun();
        ResultCode = Result.Successful;
    }

    protected static IEnumerable<Task> UnrollPagination(
            int totalCount,
            int pageSize,
            Func<Pagination, Task<int>> asyncTask,
            int startPageOffset = 1)
    {
        List<Task> tasks = [];

        int pages = totalCount / pageSize;

        for (int page = startPageOffset; page <= pages; page++)
        {
            Pagination subPagination = new()
            {
                PageSize = pageSize,
                RowOffset = page * pageSize
            };

            tasks.Add(asyncTask(subPagination));
        }

        return tasks;
    }
}
