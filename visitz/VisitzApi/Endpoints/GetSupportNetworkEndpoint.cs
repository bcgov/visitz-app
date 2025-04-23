using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetSupportNetworkEndpoint(
    string baseUrl,
    ApiRecordType type,
    string id,
    DateTimeOffset? after = null)
    : VisitzBaseEndpoint<IEnumerable<SupportNetworkJson>>(
        baseUrl,
        Vpi.V2,
        MakePath(type, id))
{
    static readonly string SupportNetworkPath = "/{0}/{1}/support-network";

    readonly DateTimeOffset? After = after;

    static string MakePath(ApiRecordType recordType, string id)
    {
        return string.Format(SupportNetworkPath, recordType.ToString().ToLowerInvariant(), id);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(after: After, pageSize: RequestParam.MaxPageSize),
        };
    }

    public override IEnumerable<SupportNetworkJson> HandleResponse(
        HttpResponseMessage response,
        string responseContent)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return [];

        JsonElement items = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty("items");

        return JsonSerializer.Deserialize<IEnumerable<SupportNetworkJson>>(items, PayloadOptions.SiebelGet);
    }
}
