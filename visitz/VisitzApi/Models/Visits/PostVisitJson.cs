using System.Text.Json.Serialization;

namespace VisitzApi.Models.Visits;

public class PostVisitJson
{
    [JsonPropertyName("Date of visit")]
    public string DateOfVisit { get; set; } = string.Empty;

    public string VisitDescription { get; set; } = string.Empty;

    [JsonPropertyName("VisitDetails")]
    public List<VisitDetailJson> VisitDetails { get; set; } = [];
}
