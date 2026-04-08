using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Visits;

internal class GetVisitsEndpoint(string baseUrl, string caseId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<VisitJson>)>(baseUrl, Vpi.V2, string.Format(VisitsPath, caseId))
{
    static readonly string VisitsPath = "/case/{0}/visits";

    readonly Pagination? Pagination = pagination;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(Pagination, @params: ("multivalue", "true")),
        };
    }

    public override (int TotalRecords, IEnumerable<VisitJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (response.GetRecordCount(), items.Deserialize<IEnumerable<VisitJson>>(PayloadOptions.SiebelGet) ?? []);
    }
}
