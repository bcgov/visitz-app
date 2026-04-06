using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisitzApi.Models.Visits;

public class VisitDetailJson
{
    [Required]
    [JsonPropertyName("Visit Detail Value")]
    public string VisitDetailValue { get; set; } = string.Empty;
}
