using System.Text.Json.Serialization;

namespace VisitzApi.Models.Base;

/// <summary>
/// Base metadata for a record. Only Id is required as it is possible for
/// records to be missing the other fields.
/// </summary>
public class BaseRecordJson
{
    [JsonRequired]
    public string Id { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public string CreatedById { get; set; } = string.Empty;

    public string UpdatedBy { get; set; } = string.Empty;

    public string UpdatedById { get; set; } = string.Empty;

    public string CreatedDate { get; set; } = string.Empty;

    public string UpdatedDate { get; set; } = string.Empty;
}
