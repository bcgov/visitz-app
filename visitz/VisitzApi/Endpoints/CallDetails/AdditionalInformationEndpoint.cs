using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.CallDetails;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.CallDetails;

internal class AdditionalInformationEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    Pagination? pagination = null
)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<AdditionalInformationJson>)>(
        baseUrl,
        Vpi.V2,
        MakePath(type, rowId)
    )
{
    static readonly string AdditionalInformationPath = "/{0}/{1}/additional-information";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(AdditionalInformationPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<AdditionalInformationJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<AdditionalInformationJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
