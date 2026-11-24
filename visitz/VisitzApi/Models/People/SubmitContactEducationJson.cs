namespace VisitzApi.Models.People;

public class SubmitContactEducationJson
{
    public required string Degree { get; set; }

    public string? SchoolName { get; set; }

    public string? StartDate { get; set; }

    public string? ContactPerson { get; set; }

    public string? ContactPersonRole { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }
}
