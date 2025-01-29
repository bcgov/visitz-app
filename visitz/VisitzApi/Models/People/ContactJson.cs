using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.People;

public class ContactJson : BaseRecordJson
{
    public string AboriginalCalc { get; set; }

    public string Age { get; set; }

    public string CaseConEndDt { get; set; }

    public string CaseConOriginalStartDt { get; set; }

    [JsonPropertyName("Case Con Parent/Caregiver")]
    public string CaseConParentCaregiver { get; set; }

    public string CaseConReportedOn { get; set; }

    public string CaseConStartDt { get; set; }

    public string CaseConSubjectChild { get; set; }

    public string CaseRelTypeCode { get; set; }

    public string CaseSubject { get; set; }

    [JsonPropertyName("Coordination AGT (CA)")]
    public string CoordinationAGTCA { get; set; }

    public string CYSNCalc { get; set; }

    [JsonPropertyName("CYSN PST Score")]
    public string CYSNPSTScore { get; set; }

    public string DateofBirth { get; set; }

    public string DependentSequenceNumber { get; set; }

    public string GivenName { get; set; }

    public string InvolvedFamilyAlerts { get; set; }

    public string Is921BandFoundCalc { get; set; }

    public string LastName { get; set; }

    public string LegalStatus { get; set; }

    [JsonPropertyName("M/F")]
    public string Sex { get; set; }

    public string SSAPrimaryField { get; set; }
}
