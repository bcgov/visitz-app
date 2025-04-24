using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

#nullable enable

internal class GetSupportNetworkEndpoint(
    string baseUrl,
    ApiRecordType type,
    string id,
    Pagination? pagination = null)
    : VisitzBaseEndpoint<IEnumerable<SupportNetworkJson>>(
        baseUrl,
        Vpi.V2,
        MakePath(type, id))
{
    static readonly string SupportNetworkPath = "/{0}/{1}/support-network";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string id)
    {
        return string.Format(SupportNetworkPath, recordType.ToString().ToLowerInvariant(), id);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(Pagination),
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

        return JsonSerializer.Deserialize<IEnumerable<SupportNetworkJson>>
            (items, PayloadOptions.SiebelGet) ?? [];
    }
}
