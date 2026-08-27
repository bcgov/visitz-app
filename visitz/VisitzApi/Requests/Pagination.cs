namespace VisitzApi.Requests;

public class Pagination
{
    public int PageSize { get; set; } = RequestParam.MaxPageSize;

    public int RowOffset { get; set; } = 0;

    public DateTimeOffset? After { get; set; }

    public Pagination NextPage(int pageNumber)
    {
        return new()
        {
            PageSize = PageSize,
            RowOffset = pageNumber * PageSize,
            After = After,
        };
    }
}
