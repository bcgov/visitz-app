using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.Activities;

public class ActivityJson : AssignableRecordJson
{
    public string AccountLocation { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string ActionBy { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;
    public string ActivityId { get; set; } = string.Empty;

    [JsonPropertyName("Activity SR Id")]
    public string ActivitySRId { get; set; } = string.Empty;
    public string ActivityTemplateId { get; set; } = string.Empty;

    [JsonPropertyName("Activity UID")]
    public string ActivityUID { get; set; } = string.Empty;
    public string Alarm { get; set; } = string.Empty;
    public string ApptAlarmTimeMin { get; set; } = string.Empty;
    public string AssetNumber { get; set; } = string.Empty;

    [JsonPropertyName("Asset Number S_ASSET")]
    public string AssetNumberS_ASSET { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string CaseNumber { get; set; } = string.Empty;
    public string CaseOwner { get; set; } = string.Empty;
    public string CaseRestricted { get; set; } = string.Empty;
    public string CaseStatus { get; set; } = string.Empty;
    public string CaseType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string ContactFirstName { get; set; } = string.Empty;
    public string ContactGivenName { get; set; } = string.Empty;
    public string ContactLastName { get; set; } = string.Empty;
    public string ContactMiddleName { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string Delay { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string Done { get; set; } = string.Empty;
    public string DoneFlag { get; set; } = string.Empty;
    public string Due { get; set; } = string.Empty;
    public string DurationMinutes { get; set; } = string.Empty;

    [JsonPropertyName("ICM Created By Office")]
    public string ICMCreatedByOffice { get; set; } = string.Empty;

    [JsonPropertyName("ICM Memo #")]
    public string ICMMemoNo { get; set; } = string.Empty;

    [JsonPropertyName("ICM Memo Id")]
    public string ICMMemoId { get; set; } = string.Empty;

    [JsonPropertyName("ICM Memo Owner")]
    public string ICMMemoOwner { get; set; } = string.Empty;

    [JsonPropertyName("ICM Memo Restricted")]
    public string ICMMemoRestricted { get; set; } = string.Empty;

    [JsonPropertyName("ICM Memo Type")]
    public string ICMMemoType { get; set; } = string.Empty;

    [JsonPropertyName("ICM Service Region")]
    public string ICMServiceRegion { get; set; } = string.Empty;

    [JsonPropertyName("ICM Sub Type")]
    public string ICMSubType { get; set; } = string.Empty;

    [JsonPropertyName("ICM Type")]
    public string ICMType { get; set; } = string.Empty;

    [JsonPropertyName("Incident #")]
    public string IncidentNo { get; set; } = string.Empty;
    public string IncidentId { get; set; } = string.Empty;

    [JsonPropertyName("MeetingLocation")]
    public string MeetingLocation { get; set; } = string.Empty;
    public string Ministry { get; set; } = string.Empty;
    public string MinistryId { get; set; } = string.Empty;
    public string Opportunity { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Overdue { get; set; } = string.Empty;
    public string OwnedBy { get; set; } = string.Empty;
    public string OwnedById { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string Planned { get; set; } = string.Empty;
    public string PreviousActivityDescription { get; set; } = string.Empty;
    public string PriceList { get; set; } = string.Empty;
    public string PrimaryContactId { get; set; } = string.Empty;
    public string PrimaryOwnedBy { get; set; } = string.Empty;
    public string PrimaryOwnerId { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Property { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
    public string RateList { get; set; } = string.Empty;
    public string Repeating { get; set; } = string.Empty;
    public string RepeatingExpires { get; set; } = string.Empty;
    public string RepeatingType { get; set; } = string.Empty;
    public string RestrictedFlag { get; set; } = string.Empty;
    public string ResultCode { get; set; } = string.Empty;
    public string RowId { get; set; } = string.Empty;
    public string Sequence { get; set; } = string.Empty;
    public string ServiceOffice { get; set; } = string.Empty;
    public string SharedFlag { get; set; } = string.Empty;
    public string ShowInContactProfile { get; set; } = string.Empty;
    public string SRNumber { get; set; } = string.Empty;
    public string SROwner { get; set; } = string.Empty;
    public string SRSubType { get; set; } = string.Empty;
    public string SRType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Updated { get; set; } = string.Empty;
    public string UpdatedByName { get; set; } = string.Empty;
}
