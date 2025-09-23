namespace VisitzApi.Requests;

internal static class RequestParam
{
    public static readonly string Since = "since";

    public static readonly string StartRowNum = "StartRowNum";

    public static readonly string PageSize = "PageSize";

    public static readonly string RecordCountNeeded = "recordcountneeded";

    public static readonly string TotalRecordCount = "total-record-count";

    public static readonly int MaxPageSize = 100;

    public static readonly string ExcludeEmptyFields = "excludeEmptyFieldsInResponse";
}
