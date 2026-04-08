using System.ComponentModel.DataAnnotations;

namespace VisitzApi.Models.SafetyAssess;

public class SubmitSafetyAssessmentHeaderJson
{
    [Required]
    public string IncidentNumber { get; set; } = string.Empty;

    [Required]
    public string DateOfAssessment { get; set; } = string.Empty;

    [Required]
    public string FamilyName { get; set; } = string.Empty;
}
