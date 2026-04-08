using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.People;

public class ContactJson : BaseRecordJson
{
    [JsonPropertyName("92_1 AGT")]
    public string _92_1AGT { get; set; } = string.Empty;

    public string ActiveAddresses { get; set; } = string.Empty;

    public string Age { get; set; } = string.Empty;

    public string AKAFirstName { get; set; } = string.Empty;

    public string AKALastName { get; set; } = string.Empty;

    public string Alerts { get; set; } = string.Empty;

    public string AutismFundingPaused { get; set; } = string.Empty;

    [JsonPropertyName("BCeID User Name")]
    public string BCeIDUserName { get; set; } = string.Empty;

    public string CanadianCitizen { get; set; } = string.Empty;

    public string CellPhone { get; set; } = string.Empty;

    public string Citizen { get; set; } = string.Empty;

    public string Citizenship { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string CollaborateID { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;

    public string ConcernsOutcome { get; set; } = string.Empty;

    [JsonPropertyName("Coordination AGT CA")]
    public string CoordinationAGTCA { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("Country of Birth")]
    public string CountryofBirth { get; set; } = string.Empty;

    public string CurrentStartDate { get; set; } = string.Empty;

    public string CYSN { get; set; } = string.Empty;

    [JsonPropertyName("Date of Birth")]
    public string DateofBirth { get; set; } = string.Empty;

    public string DateUpdated_CitizenUpdatedDate { get; set; } = string.Empty;

    public string DateUpdated_CitizenshipUpdatedDate { get; set; } = string.Empty;

    public string Deceased { get; set; } = string.Empty;

    public string DeceasedDate { get; set; } = string.Empty;

    public string EndDate { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string GivenNames { get; set; } = string.Empty;

    public string HomePhone { get; set; } = string.Empty;

    public string ImmigrationStatus { get; set; } = string.Empty;

    public string ImmigrationStatusUpdated { get; set; } = string.Empty;

    public string Indigenous { get; set; } = string.Empty;

    public string IntegrationState { get; set; } = string.Empty;

    public string InvestigationOutcomeSummary { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string LegacyDependentSequence { get; set; } = string.Empty;

    public string LegalStatus { get; set; } = string.Empty;

    public string MessagePhone { get; set; } = string.Empty;

    public string MiddleNames { get; set; } = string.Empty;

    public string OriginalStartDate { get; set; } = string.Empty;

    public string Parent_Caregiver { get; set; } = string.Empty;

    [JsonPropertyName("Person ID ICM")]
    public string PersonIDICM { get; set; } = string.Empty;

    [JsonPropertyName("Person ID MIS")]
    public string PersonIDMIS { get; set; } = string.Empty;

    [JsonPropertyName("Person Responsible for Alleged Maltreatment")]
    public string PersonResponsibleforAllegedMaltreatment { get; set; } = string.Empty;

    public string PHN { get; set; } = string.Empty;

    public string PHNVerified { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string PotentialDuplicate { get; set; } = string.Empty;

    public string PotentialDuplicateComments { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = string.Empty;

    public string Primary { get; set; } = string.Empty;

    public string PrimaryAddress { get; set; } = string.Empty;

    public string PrimaryEmail { get; set; } = string.Empty;

    public string ProjectCode { get; set; } = string.Empty;

    public string Prov { get; set; } = string.Empty;

    public string PSTScore { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string RowId { get; set; } = string.Empty;

    public string SAETPaused { get; set; } = string.Empty;

    public string SIN { get; set; } = string.Empty;

    public string StartDate { get; set; } = string.Empty;

    public string StreetAddress { get; set; } = string.Empty;

    public string StreetAddress2 { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string SubjectChild { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string UnitNumber { get; set; } = string.Empty;

    public string WorkPhone { get; set; } = string.Empty;
}
