using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Activities;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Activities;

internal class ActivitiesEndpoint(string baseUrl, ApiRecordType type, string rowId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<ActivityJson>)>(baseUrl, Vpi.V2, MakePath(type, rowId))
{
    static readonly string ActivitiesPath = "/{0}/{1}/activities";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(ActivitiesPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override (int TotalRecords, IEnumerable<ActivityJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);       

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<ActivityJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }
}
