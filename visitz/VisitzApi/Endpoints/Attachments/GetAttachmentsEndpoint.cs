using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Attachments;

internal class GetAttachmentsEndpoint(string baseUrl, ApiRecordType type, string rowId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<AttachmentJson>)>(baseUrl, Vpi.V2, MakePath(type, rowId))
{
    public static readonly string AttachmentsPath = "/{0}/{1}/attachments";

    readonly Pagination? Pagination = pagination;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(AttachmentsPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<AttachmentJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<IEnumerable<AttachmentJson>>(PayloadOptions.SiebelGet) ?? []
        );
    }
}
