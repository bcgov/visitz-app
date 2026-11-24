namespace VisitzApi.Models.People;

public class SubmitContactMedicalBehavioralJson
{
    public required string Category { get; set; }

    public string? Condition { get; set; }

    public string? DiagnosisDate { get; set; }

    public string? DiagnosedBy { get; set; }

    public string? StartDate { get; set; }

    public string? EndDate { get; set; }

    public string? TreatmentPlan { get; set; }

    public string? Comments { get; set; }
}
