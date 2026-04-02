using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisitzApi.Models.Visits;

public class VisitJson
{
    // Not using interfaces or inheritance for metadata as upstream is inconsistent

    [Required]
    public string Id { get; set; } = string.Empty;

    public string Created { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("Date of visit")]
    public string DateOfVisit { get; set; } = string.Empty;

    public string LoginName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ParentId { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Updated { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string VisitDescription { get; set; } = string.Empty;

    [JsonPropertyName("VisitDetails")]
    public List<VisitDetailJson> VisitDetails { get; set; } = [];
}
