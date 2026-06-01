using System.Net;
using System.Text.Json;
using VisitzApi.Extensions;
using VisitzApi.Json;
using VisitzApi.Models.Notes;
using VisitzApi.Requests;

namespace VisitzApi.Endpoints.Notes;

internal class GetCaseNotesEndpoint(string baseUrl, string caseId, Pagination? pagination = null)
    : VisitzBaseEndpoint<(int TotalRecords, IEnumerable<CaseNoteJson>)>(baseUrl, Vpi.V2, MakePath(caseId))
{
    static readonly string GetNotesPath = $"/{ApiRecordType.Case}/{{0}}/notes".ToLowerInvariant();

    public string EntityNumber { get; } = caseId;

    public Pagination? Pagination { get; } = pagination;

    static string MakePath(string caseId)
    {
        return string.Format(GetNotesPath, caseId);
    }

    public override HttpRequestMessage MakeRequest()
    {
        return new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = WithQueryParams(Pagination) };
    }

    public override (int TotalRecords, IEnumerable<CaseNoteJson>) HandleResponse(
        HttpResponseMessage response,
        string responseContent
    )
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return (-1, []);

        JsonElement items = JsonDocument.Parse(responseContent).RootElement.GetProperty("items");

        return (
            response.GetRecordCount(),
            items.Deserialize<List<CaseNoteJson>>(PayloadOptions.SiebelGet)?.SkipWhile(IsInvalidNote) ?? []
        );
    }

    bool IsInvalidNote(CaseNoteJson entity)
    {
        return entity.Created?.Trim().Length <= 0;
    }
}
