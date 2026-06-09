using System.Text.Json.Serialization;

namespace VisitzApi.Models.Notes;

public class CaseNoteJson : INoteJson
{
    public DateTimeOffset? ActualDateNoted { get; set; }

    public DateTimeOffset Created { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedByName { get; set; } = string.Empty;

    [JsonPropertyName("Created By Office Name")]
    public string CreatedByOffice { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public string NotePeriod { get; set; } = string.Empty;

    [JsonPropertyName("Note")]
    public string Text { get; set; } = string.Empty;

    public DateTimeOffset Updated { get; set; }

    public string UpdatedBy { get; set; } = string.Empty;

    [JsonPropertyName("Last Updated By Name")]
    public string UpdatedByName { get; set; } = string.Empty;
}
