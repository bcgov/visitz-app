using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Json;
using VisitzApi.Models.Attachments;

namespace VisitzApi.Requests;

internal class SubmitAttachmentEndpoint(string baseUrl, SubmitAttachmentEntity payload)
	: VisitzBaseEndpoint<(bool success, string attachmentId)>(baseUrl, Vpi.V1, SubmitAttachmentsPath)
{
	static readonly string SubmitAttachmentsPath = "/680";

	static readonly string RequestFormAttachmentKey = "requestFormAttachment";
	static readonly string ResponseFormAttachmentKey = "responseFormAttachment";

	static readonly string AttachmentIdKey = "attachmentId";

	static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNamingPolicy = new CamelCaseAndRemoveUnderscoreNamingPolicy()
	};

	SubmitAttachmentEntity Payload { get; } = payload;

	string RequestPayload => new JsonObject()
	{
		[RequestFormAttachmentKey] = new JsonObject()
		{
			[JsonKey.PayLoad] = JsonNode.Parse(JsonSerializer.Serialize(Payload, JsonOptions))
		}
	}.ToString();

	public override HttpRequestMessage MakeRequest()
	{
		return new HttpRequestMessage()
		{
			Content = new FormUrlEncodedContent(FormDataCollection(JsonKey.DocRequest, RequestPayload)),
			Method = HttpMethod.Post,
			RequestUri = RequestUri
		};
	}

	public override (bool success, string attachmentId) HandleResponse(string responseContent)
	{
		var rJson = JsonDocument.Parse(responseContent)
			.RootElement
			.GetProperty(ResponseFormAttachmentKey)
			.GetProperty(JsonKey.PayLoad);

		return GetProperties(rJson);
	}

	private static (bool success, string noteId) GetProperties(JsonElement json)
	{
		bool gotStatus = json.TryGetProperty(JsonKey.Status, out var status);
		bool gotNoteId = json.TryGetProperty(AttachmentIdKey, out var id);

		return gotStatus && gotNoteId
			? (status.GetString() == JsonKey.Success, id.GetString())
			: (false, null);
	}
}
