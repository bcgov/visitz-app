using VisitzApi.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace VisitzApi.Requests
{
    internal class SubmitNotesEndpoint : VisitzBaseEndpoint<(bool success, string noteId)>
    {
        private static readonly string SubmitNotesPath = "/v1/679C";

        private static readonly string RequestSubmitNotesKey = "requestSubmitNotes";
        private static readonly string ResponseSubmitNotesKey = "responseSubmitNotes";

        private static readonly string NoteIdKey = "noteId";

        public SubmitNoteEntity NoteToSubmit { get; }

        private string RequestPayload
        {
            get
            {
                return new JsonObject
                {
                    [RequestSubmitNotesKey] = new JsonObject
                    {
                        [JsonKey.Payload] = JsonSerializer.Serialize(NoteToSubmit)
                    }
                }.ToString();
            }
        }

        public SubmitNotesEndpoint(string baseUrl, SubmitNoteEntity noteToSubmit) : base(baseUrl, SubmitNotesPath)
        {
            NoteToSubmit = noteToSubmit;
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

        public override (bool success, string noteId) HandleResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            var rJson = JsonDocument.Parse(content)
                .RootElement
                .GetProperty(ResponseSubmitNotesKey)
                .GetProperty(JsonKey.Payload);

            if (rJson.TryGetProperty(JsonKey.Status, out var status) && rJson.TryGetProperty(NoteIdKey, out var id))
                return (status.GetString() == JsonKey.Success, id.GetString());
            else
                return (false, null);
        }
    }
}
