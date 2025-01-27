using System.Text.Json.Serialization;

namespace VisitzApi.Models.Base;

public class BaseRecordJson
{
    [JsonRequired]
    public string Id { get; set; }

    [JsonRequired]
    public string RowId { get; set; }

    [JsonRequired]
    public string AssignedTo { get; set; }

    [JsonRequired]
    public string AssignedToId { get; set; }

    [JsonRequired]
    public string CreatedBy { get; set; }

    [JsonRequired]
    public string CreatedById { get; set; }

    [JsonRequired]
    public string UpdatedBy { get; set; }

    [JsonRequired]
    public string UpdatedById { get; set; }

    [JsonRequired]
    public string CreatedDate { get; set; }

    [JsonRequired]
    public string UpdatedDate { get; set; }
}
