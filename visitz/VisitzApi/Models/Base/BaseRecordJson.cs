using System.Text.Json.Serialization;

namespace VisitzApi.Models.Base;

/// <summary>
/// Base metadata for a record. Only Id is required as it is possible for
/// records to be missing the other fields.
/// </summary>
public class BaseRecordJson
{
    [JsonRequired]
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public string CreatedById { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedById { get; set; }

    public string CreatedDate { get; set; }

    public string UpdatedDate { get; set; }
}
