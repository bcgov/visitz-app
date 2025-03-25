using VisitzApi.Models.Base;

namespace VisitzApi.Models.Caseload;

public class MemoJson : AssignableRecordJson
{
    public string Address { get; set; }

    public string AddressComments { get; set; }

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; }

    public string CallDate { get; set; }

    public string CallTime { get; set; }

    public string CallerAddress { get; set; }

    public string CallerEmail { get; set; }

    public string CallerName { get; set; }

    public string CallerPhone { get; set; }

    public string CellPhone { get; set; }

    public string ClosedDate { get; set; }

    public string CreatedByOffice { get; set; }

    public string GivenNames { get; set; }

    public string HomePhone { get; set; }

    public string LastName { get; set; }

    public string MedicalExamRequired { get; set; }

    public string MemoNumber { get; set; }

    public string MemoType { get; set; }

    public string Method { get; set; }

    public string NatureOfCall { get; set; }

    public string PccSummary { get; set; }

    public string PoliceForce { get; set; }

    public string PoliceInvestigation { get; set; }

    public string PoliceNotifiedDate { get; set; }

    public string PoliceReportNumber { get; set; }

    public string PreferredContactMethod { get; set; }

    public string RecordedBy { get; set; }

    public string Resolution { get; set; }

    public string RestrictedFlag { get; set; }

    public string RowId { get; set; }

    public string ServiceOffice { get; set; }

    public string Status { get; set; }

    public string TypeOfCaller { get; set; }

    public string Urgent { get; set; }
}
