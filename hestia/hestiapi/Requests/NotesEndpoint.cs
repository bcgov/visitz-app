using hestiapi.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace hestiapi.Requests
{
    internal class NotesEndpoint : HestiaBaseEndpoint<IEnumerable<NoteEntity>>
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
                        [PayloadKey] = new JsonObject()
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
                Content = new FormUrlEncodedContent(FormDataCollection(DocRequestKey, RequestPayload)),
                Method = HttpMethod.Post,
                RequestUri = RequestUri
            };
        }

        public override IEnumerable<NoteEntity> HandleResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            var notesJson = JsonDocument.Parse(content)
                .RootElement
                .GetProperty(ResponseGetNotesKey)
                .GetProperty(PayloadKey)
                .GetProperty(NotesKey);

            var notesContent = notesJson.Deserialize(typeof(List<NoteEntity>));

            return (List<NoteEntity>)notesContent;
        }
    }
}
