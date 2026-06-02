namespace VisitzApi.Models.Notes;

public interface INoteJson
{
    public DateTimeOffset Created { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedByName { get; set; }

    public string CreatedByOffice { get; set; }

    public string Id { get; set; }

    public string Text { get; set; }

    public DateTimeOffset Updated { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedByName { get; set; }
}
