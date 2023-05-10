using hestiapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace hestiapi.Requests
{
    internal class NotesEndpoint : HestiaBaseEndpoint<NotesListEntity>
    {
        private static readonly string GetNotesPath = "/v1/678";

        private static readonly string RequestGetNotesKey = "requestGetNotes";
        private static readonly string ResponseGetNotesKey = "responseGetNotes";

        private static readonly string EntityNumberKey = "entityNumber";
        private static readonly string EntityTypeKey = "entityType";

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

        public override NotesListEntity HandleResponse(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            var notesJson = JsonDocument.Parse(content)
                .RootElement
                .GetProperty(ResponseGetNotesKey)
                .GetProperty(PayloadKey);

            var notesContent = notesJson.Deserialize(typeof(NotesListEntity));

            return (NotesListEntity)notesContent;
        }
    }
}
