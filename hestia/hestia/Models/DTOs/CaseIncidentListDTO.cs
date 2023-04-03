using System;

namespace hestia.Models.DTOs
{
    public class FamilyMember
    {
        public string contactId { get; set; }
        public string keyPlayer { get; set; }
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string dateOfBirth { get; set; }
        public string sex { get; set; }
        public string relationship { get; set; }
        public string personIdICM { get; set; }
        public string legalStatus { get; set; }
        public string legalStatusEffectiveDate { get; set; }
        public string legalStatusExpiryDate { get; set; }
        public string aboriginalOrigin { get; set; }
        public string livingCommunityBand { get; set; }
        public string email { get; set; }
        public string homePhone { get; set; }
        public string cellPhone { get; set; }
        public string subjectFlag { get; set; }
        public string parentCaregiver { get; set; }
        public string subjectChild { get; set; }
        public string personResponsibleMaltreatment { get; set; }
        public string contactUnitNo { get; set; }
        public string contactAddressLine1 { get; set; }
        public string contactAddressLine2 { get; set; }
        public string contactCity { get; set; }
        public string contactPostalCode { get; set; }
        public string contactProvinceState { get; set; }
        public string contactCountry { get; set; }
    }

    public class ListCaseIncident
    {
        public PayLoad payLoad { get; set; }
    }

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

    public class PayLoad
    {
        public List<ListCaseIncident2> listCaseIncidents { get; set; }
    }

    /// <summary>
    /// The data transfer object that would be used by the networking module to deserialize response.
    /// </summary>
    public class CaseIncidentListDTO
    {
        public ListCaseIncident listCaseIncident { get; set; }
    }
}

