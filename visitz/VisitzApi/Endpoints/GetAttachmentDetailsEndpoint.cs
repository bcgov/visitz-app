using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Attachments;
using VisitzApi.Models.People;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetAttachmentDetailsEndpoint(string baseUrl, ApiRecordType type, string rowId, DateTimeOffset? after = null)
    : VisitzBaseEndpoint<IEnumerable<AttachmentListJson>>(baseUrl, Vpi.V2, MakePath(type, rowId))
{
    static readonly string AttachmentsPath = "/{0}/{1}/attachment-details";

    readonly DateTimeOffset? After = after;

    static string MakePath(ApiRecordType recordType, string rowId)
    {
        return string.Format(AttachmentsPath, recordType.ToString().ToLowerInvariant(), rowId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(after: After, pageSize: RequestParam.MaxPageSize),
        };
    }

    public override IEnumerable<AttachmentListJson> HandleResponse(HttpResponseMessage response, string responseContent)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return [];

        JsonElement items = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty("items");

        return JsonSerializer.Deserialize<IEnumerable<AttachmentListJson>>(items, PayloadOptions.SiebelGet);
    }
}
