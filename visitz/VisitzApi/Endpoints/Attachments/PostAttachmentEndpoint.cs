using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Attachments;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Attachments;

internal class PostAttachmentEndpoint(string baseUrl, ApiRecordType type, string recordId, AttachmentFormData data)
    : VisitzBaseEndpoint<(bool TotalCount, string AttachmentId)>(baseUrl, Vpi.V2, MakePath(type, recordId))
{
    readonly AttachmentFormData data = data;

    static string MakePath(ApiRecordType type, string recordId)
    {
        return string.Format(GetAttachmentsEndpoint.AttachmentsPath, type.ToString().ToLowerInvariant(), recordId);
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

    public override (bool TotalCount, string AttachmentId) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        string attachmentId = "";

        try
        {
            var root = JsonDocument.Parse(responseContent).RootElement;

            if (root.FindFirstByName(JsonKey.Id) is JsonElement found)
                attachmentId = found.GetString() ?? "";
        }
        catch (Exception)
        { /* not throwing exception since API call was actually successful */
        }

        return (response.IsSuccessStatusCode, attachmentId);
    }
}
