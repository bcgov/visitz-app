using System.Text.Json.Serialization;

namespace VisitzApi.Models.Visits;

public class PostVisitJson
{
    [JsonPropertyName("Date of visit")]
    public DateTimeOffset DateOfVisit { get; set; }

    public string VisitDescription { get; set; }

    [JsonPropertyName("VisitDetails")]
    public List<VisitDetailJson> VisitDetails { get; set; }
}
