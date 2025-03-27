using System.Text.Json;
using VisitzApi.Json;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints;

internal class GetAttachmentDetailsEndpoint(string baseUrl, ApiRecordType type, string rowId, string attachmentId, DateTimeOffset? after = null)
    : VisitzBaseEndpoint<AttachmentJson>(baseUrl, Vpi.V2, MakePath(type, rowId, attachmentId))
{
    static readonly string AttachmentsPath = "/{0}/{1}/attachments/{2}";

    readonly DateTimeOffset? After = after;

    static string MakePath(ApiRecordType recordType, string rowId, string attachmentId)
    {
        return string.Format(AttachmentsPath, recordType.ToString().ToLowerInvariant(), rowId, attachmentId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = WithQueryParams(after: After),
        };

        request.Headers.Add("Accept", "*/*");
        return request;
    }

    public override AttachmentJson HandleResponse(HttpResponseMessage response, string responseContent)
    {
        return JsonSerializer.Deserialize<AttachmentJson>(responseContent, PayloadOptions.SiebelGet);
    }
}
