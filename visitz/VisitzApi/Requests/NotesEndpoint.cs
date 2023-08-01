using VisitzApi.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace VisitzApi.Requests
{
    internal class NotesEndpoint : VisitzBaseEndpoint<IEnumerable<NoteEntity>>
    {
        private static readonly string GetNotesPath = "/v1/678";

        private static readonly string RequestGetNotesKey = "requestGetNotes";
        private static readonly string ResponseGetNotesKey = "responseGetNotes";

        private static readonly string EntityNumberKey = "entityNumber";
        private static readonly string EntityTypeKey = "entityType";
        private static readonly string NotesKey = "notes";

        public string EntityNumber { get; }
        public string EntityType { get; }

        private string RequestPayload
        {
            get
            {
                return new JsonObject()
                {
                    [RequestGetNotesKey] = new JsonObject()
                    {
                        [JsonKey.Payload] = new JsonObject()
                        {
                            [EntityNumberKey] = EntityNumber,
                            [EntityTypeKey] = EntityType
                        }
                    }
                }.ToString();
            }
        }

        public NotesEndpoint(string baseUrl, string entityNumber, string entityType) : base(baseUrl, GetNotesPath)
        {
            EntityNumber = entityNumber;
            EntityType = entityType;
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

        public override IEnumerable<NoteEntity> HandleResponse(string responseContent)
        {
            var notesJson = JsonDocument.Parse(responseContent)
                .RootElement
                .GetProperty(ResponseGetNotesKey)
                .GetProperty(JsonKey.Payload)
                .GetProperty(NotesKey);

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var notesContent = (List<NoteEntity>)notesJson.Deserialize(typeof(List<NoteEntity>), options);

            return notesContent.SkipWhile(IsInvalidNote);
        }

        private bool IsInvalidNote(NoteEntity entity)
        {
            return entity.CreatedDate?.Trim().Length <= 0;
        }
    }
}
