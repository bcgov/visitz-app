using System.Text.Json.Serialization;

namespace VisitzApi.Models
{
    public class CaseloadEntity
    {
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
        public List<FamilyMemberEntity> FamilyMembers { get; set; }
        public string KeyPlayerCellPhone { get; set; }
        public string KeyPlayerEmail { get; set; }
        public string KeyPlayerHomePhone { get; set; }
        public string CreatedDate { get; set; }
        public string DateReported { get; set; }
    }
}
