using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using VisitzApi.Json;
using VisitzApi.Models;

namespace VisitzApi.Endpoints.Notes;

internal class SubmitNotesEndpoint(string baseUrl, SubmitNoteEntity noteToSubmit)
    : VisitzBaseEndpoint<(bool success, string noteId)>(baseUrl, Vpi.V2, SubmitNotesPath)
{
    private static readonly string SubmitNotesPath = "/wf/submit-notes";

    private static readonly string RequestSubmitNotesKey = "RequestSubmitNotes";
    private static readonly string NoteIdKey = "NoteId";

    public SubmitNoteEntity NoteToSubmit { get; } = noteToSubmit;

    private JsonObject RequestPayload
    {
        get { return new JsonObject { [RequestSubmitNotesKey] = new JsonArray { NoteToSubmit } }; }
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage()
        {
            Content = JsonContent.Create(RequestPayload),
            Method = HttpMethod.Post,
            RequestUri = RequestUri,
        };
    }

    public override (bool success, string noteId) HandleResponse(HttpResponseMessage _, string responseContent)
    {
        var rJson = JsonDocument.Parse(responseContent).RootElement;

        return GetProperties(rJson);
    }

    private static (bool success, string noteId) GetProperties(JsonElement json)
    {
        string? status = json.GetProperty(JsonKey.Status).GetString();
        string noteId = json.GetProperty(NoteIdKey).GetString() ?? "";

        return (status == JsonKey.Success, noteId);
    }
}
