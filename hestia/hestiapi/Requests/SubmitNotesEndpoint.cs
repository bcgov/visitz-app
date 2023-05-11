using visitzApi.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace visitzApi.Requests
{
    internal class SubmitNotesEndpoint : VisitzBaseEndpoint<HttpStatusCode>
    {
        private static readonly string SubmitNotesPath = "/v1/679";

        public SubmitNoteEntity NoteToSubmit { get; }

        private string RequestPayload
        {
            get
            {
                return new JsonObject
                {
                    ["requestSubmitNotes"] = new JsonObject
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

        public override HttpStatusCode HandleResponse(HttpResponseMessage response)
        {
            // TODO: Implement!
            throw new NotImplementedException();
        }
    }
}
