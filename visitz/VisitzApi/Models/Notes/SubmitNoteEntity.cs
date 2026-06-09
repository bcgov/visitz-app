using System.Text.Json.Serialization;

namespace VisitzApi.Models.Notes;

public class SubmitNoteEntity
{
    [JsonPropertyName("entityNumber")]
    public string EntityNumber { get; set; } = string.Empty;

    [JsonPropertyName("entityType")]
    public string EntityType { get; set; } = string.Empty;

    [JsonPropertyName("notePeriod")]
    public string NotePeriod { get; set; } = string.Empty;

    [JsonPropertyName("notes")]
    public string Content { get; set; } = string.Empty;
}
