using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisitzApi.Models.Visits;

public class VisitJson
{
    // Not using interfaces or inheritance for metadata as upstream is inconsistent

    [Required]
    public string Id { get; set; }

    public string Created { get; set; }

    public string CreatedBy { get; set; }

    [JsonPropertyName("Date of visit")]
    public string DateOfVisit { get; set; }

    public string LoginName { get; set; }

    public string Name { get; set; }

    public string ParentId { get; set; }

    public string Type { get; set; }

    public string Updated { get; set; }

    public string UpdatedBy { get; set; }

    public string VisitDescription { get; set; }

    public string VisitDetailsValue { get; set; }
}
