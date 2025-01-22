using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Json;
using VisitzApi.Models;

namespace VisitzApi.Requests
{
	internal class SubmitNotesEndpoint(string baseUrl, SubmitNoteEntity noteToSubmit)
        : VisitzBaseEndpoint<(bool success, string noteId)>(baseUrl, Vpi.V1, SubmitNotesPath)
    {
        private static readonly string SubmitNotesPath = "/679C";

        private static readonly string RequestSubmitNotesKey = "requestSubmitNotes";
        private static readonly string ResponseSubmitNotesKey = "responseSubmitNotes";

        private static readonly string NoteIdKey = "noteId";

        public SubmitNoteEntity NoteToSubmit { get; } = noteToSubmit;

        private string RequestPayload
        {
            get
            {
                return new JsonObject
                {
                    [RequestSubmitNotesKey] = new JsonObject
                    {
                        [JsonKey.PayLoad] = JsonNode.Parse(JsonSerializer.Serialize(NoteToSubmit))
                    }
                }.ToString();
            }
        }

        public override HttpRequestMessage MakeRequest()
        {
            return new HttpRequestMessage()
            {
                Content = new FormUrlEncodedContent(FormDataCollection(JsonKey.DocRequest, RequestPayload)),
                Method = HttpMethod.Post,
                RequestUri = RequestUri
            };
        }

        public override (bool success, string noteId) HandleResponse(string responseContent)
        {
            var rJson = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty(ResponseSubmitNotesKey)
                .GetProperty(JsonKey.PayLoad);

            return GetProperties(rJson);
        }

        private static (bool success, string noteId) GetProperties(JsonElement json)
        {
            bool gotStatus = json.TryGetProperty(JsonKey.Status, out var status);
            bool gotNoteId = json.TryGetProperty(NoteIdKey, out var id);

            return gotStatus && gotNoteId
                ? (status.GetString() == JsonKey.Success, id.GetString())
                : (false, null);
        }
    }
}
