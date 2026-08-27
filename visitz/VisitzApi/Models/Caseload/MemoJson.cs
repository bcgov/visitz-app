using VisitzApi.Models.Base;

namespace VisitzApi.Models.Caseload;

public class MemoJson : AssignableRecordJson
{
    public string Address { get; set; } = string.Empty;

    public string AddressComments { get; set; } = string.Empty;

    public string AreAnyOfTheFamilyMembersIndigenous { get; set; } = string.Empty;

    public string CallDate { get; set; } = string.Empty;

    public string CallTime { get; set; } = string.Empty;

    public string CallerAddress { get; set; } = string.Empty;

    public string CallerEmail { get; set; } = string.Empty;

    public string CallerName { get; set; } = string.Empty;

    public string CallerPhone { get; set; } = string.Empty;

    public string CellPhone { get; set; } = string.Empty;

    public string ClosedDate { get; set; } = string.Empty;

    public string CreatedByOffice { get; set; } = string.Empty;

    public string GivenNames { get; set; } = string.Empty;

    public string HomePhone { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string MedicalExamRequired { get; set; } = string.Empty;

    public string MemoNumber { get; set; } = string.Empty;

    public string MemoType { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public string NatureOfCall { get; set; } = string.Empty;

    public string PccSummary { get; set; } = string.Empty;

    public string PoliceForce { get; set; } = string.Empty;

    public string PoliceInvestigation { get; set; } = string.Empty;

    public string PoliceNotifiedDate { get; set; } = string.Empty;

    public string PoliceReportNumber { get; set; } = string.Empty;

    public string PreferredContactMethod { get; set; } = string.Empty;

    public string RecordedBy { get; set; } = string.Empty;

    public string Resolution { get; set; } = string.Empty;

    public string RestrictedFlag { get; set; } = string.Empty;

    public string RowId { get; set; } = string.Empty;

    public string ServiceOffice { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string TypeOfCaller { get; set; } = string.Empty;

    public string Urgent { get; set; } = string.Empty;
}
