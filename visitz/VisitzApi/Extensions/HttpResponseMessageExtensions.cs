using VisitzApi.Requests;

namespace VisitzApi.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static int GetRecordCount(this HttpResponseMessage message)
    {
        return message.Headers.TryGetValues(RequestParam.TotalRecordCount, out var recordCount)
            ? GetCount(recordCount)
            : -1;
    }

    static int GetCount(IEnumerable<string> values)
    {
        foreach (var value in values)
            if (int.TryParse(value, out int count))
                return count;

        return -1;
    }
}
