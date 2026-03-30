using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.People;
using VisitzApi.Requests;
#nullable enable
namespace VisitzApi.Endpoints.People;

internal class ContactLanguagesEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    string contactrowId,
    Pagination? pagination = null
)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<ContactLanguageJson>)>(
        baseUrl,
        Vpi.V2,
        MakePath(type, rowId, contactrowId)
    )
{
    static readonly string contactsPath = "/{0}/{1}/contact/{2}/languages";
    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId, string contactrowId)
    {
        return string.Format(contactsPath, recordType.ToString().ToLowerInvariant(), rowId, contactrowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<ContactLanguageJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<ContactLanguageJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
