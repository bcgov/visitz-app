using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.CallDetails;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.CallDetails;

internal class IncidentConcernsEndpoint(string baseUrl, string incidentId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<IncidentConcernsJson>)>(
        baseUrl,
        Vpi.V2,
        string.Format(IncidentsPath, incidentId)
    )
{
    static readonly string IncidentsPath = "/incident/{0}/concerns";

    readonly Pagination? Pagination = pagination;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<IncidentConcernsJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<IncidentConcernsJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
