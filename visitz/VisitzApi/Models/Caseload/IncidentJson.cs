using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.Caseload;

public class IncidentJson : AssignableRecordJson
{
    public string ActivityUID { get; set; }

    public string AfterHoursFlag { get; set; }

    public string ContactFirstName { get; set; }

    public string ContactLastName { get; set; }

    public string ContactMiddleName { get; set; }

    public string DateClosed { get; set; }

    public string DateCreated { get; set; }

    public string DateOccurred { get; set; }

    public string DateReported { get; set; }

    public string DaysOpen { get; set; }

    public string Description { get; set; }

    public string Display { get; set; }

    public string ICMServiceRegion { get; set; }

    public string ICMServiceRegionCode { get; set; }

    public string IncidentCity { get; set; }

    public string IncidentLocation { get; set; }

    public string IncidentPostalCode { get; set; }

    public string IncidentSubType { get; set; }

    public string IncidentType { get; set; }

    public string IntegrationErrorDescription { get; set; }

    public string IntegrationState { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public string Organization { get; set; }

    public string OwnedBy { get; set; }

    public string Planned { get; set; }

    public string PrimarySuspectId { get; set; }

    public string Priority { get; set; }

    public string Resolution { get; set; }

    public string ResponseTime { get; set; }

    public string RestrictedFlag { get; set; }

    public string RowStatusOld { get; set; }

    public string ServiceOffice { get; set; }

    public string SourceId { get; set; }

    public string Status { get; set; }

    public string SubStatus { get; set; }

    public string SubSubType { get; set; }

    public string SuppressCalendar { get; set; }

    public string SystemAsgnFlag { get; set; }

    public string TemplateFlag { get; set; }
}
