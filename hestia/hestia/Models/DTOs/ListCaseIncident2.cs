namespace hestia.Models.DTOs
{
    public class ListCaseIncident2
    {
        public string caseIncidentNumber { get; set; }
        public string entityType { get; set; }
        public string caseIncidentType { get; set; }
        public string workerId { get; set; }
        public string workerFullName { get; set; }
        public string dateReported { get; set; }
        public string serviceOffice { get; set; }
        public string officeCode { get; set; }
        public string safetyAssessmentExist { get; set; }
        public string unitNo { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string provinceState { get; set; }
        public string country { get; set; }
        public List<FamilyMember> familyMembers { get; set; }
        public string keyPlayerHomePhone { get; set; }
        public string keyPlayerCellPhone { get; set; }
        public string keyPlayerEmail { get; set; }
        public string teamLeadsFullName { get; set; }
        public string createdDate { get; set; }
    }
}

