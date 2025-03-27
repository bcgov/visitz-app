using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.People;

public class ContactJson : BaseRecordJson
{
    [JsonPropertyName("92_1 AGT")]
    public string _92_1AGT { get; set; }

    public string ActiveAddresses { get; set; }

    public string Age { get; set; }

    public string AKAFirstName { get; set; }

    public string AKALastName { get; set; }

    public string Alerts { get; set; }

    public string AutismFundingPaused { get; set; }

    [JsonPropertyName("BCeID User Name")]
    public string BCeIDUserName { get; set; }

    public string CanadianCitizen { get; set; }

    public string CellPhone { get; set; }

    public string Citizen { get; set; }

    public string Citizenship { get; set; }

    public string City { get; set; }

    public string CollaborateID { get; set; }

    public string Comments { get; set; }

    public string ConcernsOutcome { get; set; }

    [JsonPropertyName("Coordination AGT CA")]
    public string CoordinationAGTCA { get; set; }

    public string Country { get; set; }

    [JsonPropertyName("Country of Birth")]
    public string CountryofBirth { get; set; }

    public string CurrentStartDate { get; set; }

    public string CYSN { get; set; }

    [JsonPropertyName("Date of Birth")]
    public string DateofBirth { get; set; }

    public string DateUpdated_CitizenUpdatedDate { get; set; }

    public string DateUpdated_CitizenshipUpdatedDate { get; set; }

    public string Deceased { get; set; }

    public string DeceasedDate { get; set; }

    public string EndDate { get; set; }

    public string FirstName { get; set; }

    public string Gender { get; set; }

    public string GivenNames { get; set; }

    public string HomePhone { get; set; }

    public string ImmigrationStatus { get; set; }

    public string ImmigrationStatusUpdated { get; set; }

    public string Indigenous { get; set; }

    public string IntegrationState { get; set; }

    public string InvestigationOutcomeSummary { get; set; }

    public string LastName { get; set; }

    public string LegacyDependentSequence { get; set; }

    public string LegalStatus { get; set; }

    public string MessagePhone { get; set; }

    public string MiddleNames { get; set; }

    public string OriginalStartDate { get; set; }

    public string Parent_Caregiver { get; set; }

    [JsonPropertyName("Person ID ICM")]
    public string PersonIDICM { get; set; }

    [JsonPropertyName("Person ID MIS")]
    public string PersonIDMIS { get; set; }

    [JsonPropertyName("Person Responsible for Alleged Maltreatment")]
    public string PersonResponsibleforAllegedMaltreatment { get; set; }

    public string PHN { get; set; }

    public string PHNVerified { get; set; }

    public string PostalCode { get; set; }

    public string PotentialDuplicate { get; set; }

    public string PotentialDuplicateComments { get; set; }

    public string PreferredLanguage { get; set; }

    public string Primary { get; set; }

    public string PrimaryAddress { get; set; }

    public string PrimaryEmail { get; set; }

    public string ProjectCode { get; set; }

    public string Prov { get; set; }

    public string PSTScore { get; set; }

    public string Relationship { get; set; }

    public string Role { get; set; }

    public string RowId { get; set; }

    public string SAETPaused { get; set; }

    public string SIN { get; set; }

    public string StartDate { get; set; }

    public string StreetAddress { get; set; }

    public string StreetAddress2 { get; set; }

    public string Subject { get; set; }

    public string SubjectChild { get; set; }

    public string Title { get; set; }

    public string UnitNumber { get; set; }

    public string WorkPhone { get; set; }
}
