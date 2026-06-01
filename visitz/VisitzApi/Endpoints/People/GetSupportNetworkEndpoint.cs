using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.People;

internal class GetSupportNetworkEndpoint(string baseUrl, ApiRecordType type, string id, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<SupportNetworkJson>)>(baseUrl, Vpi.V2, MakePath(type, id))
{
    static readonly string SupportNetworkPath = "/{0}/{1}/support-network";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string id)
    {
        return string.Format(SupportNetworkPath, recordType.ToString().ToLowerInvariant(), id);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<SupportNetworkJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<SupportNetworkJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
