using VisitzApi.Models.Base;

namespace VisitzApi.Models.Caseload;

public class IncidentJson : AssignableRecordJson
{
    public string AddressComments { get; set; }

    public string Address { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string Caseload { get; set; }

    public string CellPhone { get; set; }

    public string ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public string DateReported { get; set; }

    public string GivenNames { get; set; }

    public string HomePhone { get; set; }

    public string IncidentNumber { get; set; }

    public string LastName { get; set; }

    public string MedicalExamRequired { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PoliceForce { get; set; }

    public string PoliceInvestigation { get; set; }

    public string PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; }

    public string PreferredContactMethod { get; set; }

    public string ProtectionResponse { get; set; }

    public string Resolution { get; set; }

    public string ResponsePriority { get; set; }

    public string RestrictedFlag { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    public string Type { get; set; }

    public string TypeOfCaller { get; set; }
}
