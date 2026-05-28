using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Json;
using VisitzApi.Models;

namespace VisitzApi.Endpoints.Notes;

internal class GetNotesEndpoint(string baseUrl, string entityNumber, string entityType)
    : VisitzBaseEndpoint<IEnumerable<NoteEntity>>(baseUrl, Vpi.V1, GetNotesPath)
{
    private static readonly string GetNotesPath = "/678";

    private static readonly string RequestGetNotesKey = "requestGetNotes";
    private static readonly string ResponseGetNotesKey = "responseGetNotes";

    private static readonly string EntityNumberKey = "entityNumber";
    private static readonly string EntityTypeKey = "entityType";
    private static readonly string NotesKey = "notes";

    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public string EntityNumber { get; } = entityNumber;
    public string EntityType { get; } = entityType;

    private string RequestPayload
    {
        get
        {
            return new JsonObject()
            {
                [RequestGetNotesKey] = new JsonObject()
                {
                    [JsonKey.PayLoad] = new JsonObject()
                    {
                        [EntityNumberKey] = EntityNumber,
                        [EntityTypeKey] = EntityType,
                    },
                },
            }.ToString();
        }
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Content = new FormUrlEncodedContent(FormDataCollection(JsonKey.DocRequest, RequestPayload)),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override IEnumerable<NoteEntity> HandleResponse(HttpResponseMessage _, string responseContent)
    {
        var notesJson = JsonDocument
            .Parse(responseContent)
            .RootElement.GetProperty(ResponseGetNotesKey)
            .GetProperty(JsonKey.PayLoad)
            .GetProperty(NotesKey);

        return notesJson.Deserialize<List<NoteEntity>>(Options)?.SkipWhile(IsInvalidNote) ?? [];
    }

    private bool IsInvalidNote(NoteEntity entity)
    {
        return entity.CreatedDate?.Trim().Length <= 0;
    }
}
