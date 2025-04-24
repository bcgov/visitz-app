using System.Net;
using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

#nullable enable

internal class GetAttachmentsEndpoint(
    string baseUrl,
    ApiRecordType type,
    string rowId,
    DateTimeOffset? after = null)
    : VisitzBaseEndpoint<IEnumerable<AttachmentJson>>(
        baseUrl,
        Vpi.V2,
        MakePath(type, rowId))
{
    static readonly string AttachmentsPath = "/{0}/{1}/attachments";

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

    public override IEnumerable<AttachmentJson> HandleResponse(HttpResponseMessage response, string responseContent)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return [];

        JsonElement items = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty("items");

        return JsonSerializer.Deserialize<IEnumerable<AttachmentJson>>
            (items, PayloadOptions.SiebelGet) ?? [];
    }
}
