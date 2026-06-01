using VisitzApi.Models.Base;

namespace VisitzApi.Models.People;

public class SupportNetworkJson : BaseRecordJson
{
    public string Active { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Agency { get; set; } = string.Empty;

    public string Cell { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;
}
