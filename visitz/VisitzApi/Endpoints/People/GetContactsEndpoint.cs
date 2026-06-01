using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.People;

internal class GetContactsEndpoint(string baseUrl, ApiRecordType type, string rowId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<ContactJson>)>(baseUrl, Vpi.V2, MakePath(type, rowId))
{
    static readonly string ContactsPath = "/{0}/{1}/contacts";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(ContactsPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<ContactJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (response.GetRecordCount(), items.Deserialize<IEnumerable<ContactJson>>(PayloadOptions.SiebelGet) ?? []);
    }
}
