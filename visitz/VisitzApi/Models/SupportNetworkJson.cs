using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models;

public class SupportNetworkJson : BaseRecordJson
{
    public string Active { get; set; }

    public string Address { get; set; }

    public string AgencyName { get; set; }

    public string CellPhoneNumber { get; set; }

    public string Comments { get; set; }

    public string EmergencyContact { get; set; }

    public string EntityId { get; set; }

    public string EntityName { get; set; }

    [JsonPropertyName("ICM SNC Case Con Flag")]
    public string ICMSNCCaseConFlag { get; set; }

    [JsonPropertyName("ICM SNC SR Con Flag")]
    public string ICMSNCSRConFlag { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Relationship { get; set; }
}
