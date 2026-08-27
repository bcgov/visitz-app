using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Notes;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Notes;

internal class GetResponseNarrativeEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    Pagination? pagination = null
) : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<ResponseNarrativeJson>)>(baseUrl, Vpi.V2, MakePath(type, rowId))
{
    const string NarrativesPath = "/{0}/{1}/response-narratives";

    public Pagination? Pagination { get; set; } = pagination;

    static string MakePath(ApiRecordType type, string rowId)
    {
        if (type is ApiRecordType.Incident or ApiRecordType.SR)
            return string.Format(NarrativesPath, type.ToString().ToLowerInvariant(), rowId);
        else
            throw new InvalidOperationException($"Invalid type '{type}' provided");
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<ResponseNarrativeJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<ResponseNarrativeJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
