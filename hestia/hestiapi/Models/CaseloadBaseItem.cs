using System.Text.Json.Serialization;

namespace hestiapi.Models
{
    [JsonDerivedType(typeof(CaseloadCaseEntity), CaseEntityKey)]
    [JsonDerivedType(typeof(CaseloadIncidentEntity), IncidentEntityKey)]
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
    public class CaseloadBaseItem
    {
        private const string CaseEntityKey = "Case";
        private const string IncidentEntityKey = "Incident";

        public string EntityType { get; set; }
        public string CaseIncidentNumber { get; set; }
        public string CaseIncidentType { get; set; }
        public string WorkerId { get; set; }
        public string WorkerFullName { get; set; }
        public string ServiceOffice { get; set; }
        public string OfficeCode { get; set; }
        public string SafetyAssessmentExist { get; set; }
        public string UnitNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string ProvinceState { get; set; }
        public string Country { get; set; }
        public string FamilyMembers { get; set; }
    }
}
