using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetVisitsEndpoint(string baseUrl, string caseId, DateTimeOffset? after = null)
    : VisitzBaseEndpoint<IEnumerable<VisitJson>>(baseUrl, Vpi.V2, string.Format(VisitsPath, caseId))
{
    static readonly string VisitsPath = "/case/{0}/visits";

    readonly DateTimeOffset? After = after;

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(after: After),
        };
    }

    public override IEnumerable<VisitJson> HandleResponse(HttpResponseMessage response, string responseContent)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return [];

        JsonElement items = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty("items");

        return JsonSerializer.Deserialize<IEnumerable<VisitJson>>(items, PayloadOptions.SiebelGet);
    }
}
