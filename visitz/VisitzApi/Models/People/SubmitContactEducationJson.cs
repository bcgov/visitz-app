namespace VisitzApi.Models.People;

public class SubmitContactEducationJson
{
    public required string Degree { get; set; }

    public string? Comments { get; set; }

    public string? DateLastAttended { get; set; }

    public string? EndDate { get; set; }

    public string? IndividualEducationPlan { get; set; }

    public string? LearningAssistant { get; set; }

    public string? StartDate { get; set; }
}
