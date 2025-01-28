using System.Text.Json;
using VisitzApi.Models.Visits;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetVisitsEndpoint(string baseUrl, string caseId, DateTimeOffset? after = null)
    : VisitzBaseEndpoint<IEnumerable<VisitJson>>(baseUrl, Vpi.V2, string.Format(VisitsPath, caseId))
{
    static readonly string VisitsPath = "/{0}/visits";

    readonly DateTimeOffset? After = after;

    public override HttpRequestMessage MakeRequest()
    {
        Uri uri = RequestUri;

        if (After is DateTimeOffset after)
            uri = new Uri(uri, AfterParam(after));

        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = uri
        };
    }

    public override IEnumerable<VisitJson> HandleResponse(string responseContent)
    {
        JsonElement items = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty("items");

        return JsonSerializer.Deserialize<IEnumerable<VisitJson>>(items);
    }
}
