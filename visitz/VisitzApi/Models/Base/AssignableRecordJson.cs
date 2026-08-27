namespace VisitzApi.Models.Base;

public class AssignableRecordJson : BaseRecordJson
{
    public string AssignedTo { get; set; } = string.Empty;

    public string AssignedToId { get; set; } = string.Empty;
}
