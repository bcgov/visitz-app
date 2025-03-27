using VisitzApi.Models.Base;

namespace VisitzApi.Models;

public class SupportNetworkJson : BaseRecordJson
{
    public string Active { get; set; }

    public string Address { get; set; }

    public string Agency { get; set; }

    public string Cell { get; set; }

    public string Comments { get; set; }

    public string EntityId { get; set; }

    public string EntityName { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Relationship { get; set; }
}
