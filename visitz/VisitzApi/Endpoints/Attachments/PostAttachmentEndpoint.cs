using VisitzApi.Models.Attachments;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Attachments;

internal class PostAttachmentEndpoint(
    string baseUrl,
    ApiRecordType type,
    string recordId,
    AttachmentFormData data)
    : VisitzBaseEndpoint<bool>(baseUrl, Vpi.V2, MakePath(type, recordId))
{
    readonly AttachmentFormData data = data;

    static string MakePath(ApiRecordType type, string recordId)
    {
        return string.Format(
            GetAttachmentsEndpoint.AttachmentsPath,
            type.ToString().ToLowerInvariant(),
            recordId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
            Content = data.ToFormDataContent(),
        };
    }

    public override bool HandleResponse(HttpResponseMessage response, string responseContent)
    {
        return response.IsSuccessStatusCode;
    }
}
