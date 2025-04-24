namespace VisitzApi.Requests;

#nullable enable

public class Pagination
{
    public int PageSize { get; set; } = RequestParam.MaxPageSize;

    public int RowOffset { get; set; } = 0;

    public DateTimeOffset? After { get; set; }
}
