using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

#nullable enable

internal class GetContactsEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    Pagination? pagination = null)
    : VisitzBaseEndpoint<IEnumerable<ContactJson>>(
        baseUrl,
        Vpi.V2,
        MakePath(type, rowId))
{
    static readonly string ContactsPath = "/{0}/{1}/contacts";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(ContactsPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(Pagination),
        };
    }

    public override IEnumerable<ContactJson> HandleResponse(HttpResponseMessage response, string responseContent)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return [];

        JsonElement items = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty("items");

        return JsonSerializer.Deserialize<IEnumerable<ContactJson>>
            (items, PayloadOptions.SiebelGet) ?? [];
    }
}
