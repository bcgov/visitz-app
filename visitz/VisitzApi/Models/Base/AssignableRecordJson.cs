using System.Text.Json.Serialization;

namespace VisitzApi.Models.Base;

public class AssignableRecordJson : BaseRecordJson
{
    [JsonRequired]
    public string AssignedTo { get; set; }

    [JsonRequired]
    public string AssignedToId { get; set; }
}
