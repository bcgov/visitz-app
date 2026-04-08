using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.SafetyAssess;

public class GetSafetyAsessmentJson : BaseRecordJson
{
    public class ChildContacts
    {
        public string Id { get; set; } = string.Empty;
    }

    public string ApprovedBy { get; set; } = string.Empty;

    public string ApprovedDate { get; set; } = string.Empty;

    public string ApprovedToFinalize { get; set; } = string.Empty;

    public string ApprovedToFinalizeDate { get; set; } = string.Empty;

    public string ApprovedToFinalizeDS { get; set; } = string.Empty;

    public string ChildContactId { get; set; } = string.Empty;

    public IList<ChildContacts> ContactsInOutCare { get; set; } = [];

    public string DataStewardRole { get; set; } = string.Empty;

    public string DateOfAssessment { get; set; } = string.Empty;

    public string FactorInfluence1 { get; set; } = string.Empty;

    public string FactorInfluence2 { get; set; } = string.Empty;

    public string FactorInfluence3 { get; set; } = string.Empty;

    public string FactorInfluence4 { get; set; } = string.Empty;

    public string FactorInfluence5 { get; set; } = string.Empty;

    public string FamilyName { get; set; } = string.Empty;

    public string FinalizedDate { get; set; } = string.Empty;

    public string ProtectiveCapacity01 { get; set; } = string.Empty;

    public string ProtectiveCapacity02 { get; set; } = string.Empty;

    public string ProtectiveCapacity03 { get; set; } = string.Empty;

    public string ProtectiveCapacity04 { get; set; } = string.Empty;

    public string ProtectiveCapacity05 { get; set; } = string.Empty;

    public string ProtectiveCapacity06 { get; set; } = string.Empty;

    public string ProtectiveCapacity07 { get; set; } = string.Empty;

    public string ProtectiveCapacity08 { get; set; } = string.Empty;

    public string ProtectiveCapacity09 { get; set; } = string.Empty;

    public string ProtectiveCapacity10 { get; set; } = string.Empty;

    public string ProtectiveCapacity11 { get; set; } = string.Empty;

    public string ProtectiveCapacity12 { get; set; } = string.Empty;

    public string ProtectiveCapacity12Other { get; set; } = string.Empty;

    public string ProtectiveCapacityObservations { get; set; } = string.Empty;

    public string ReadyToFinalize { get; set; } = string.Empty;

    public string ReadyToFinalizeDate { get; set; } = string.Empty;

    public string RowId { get; set; } = string.Empty;

    public string SafetyDecisionIntervention { get; set; } = string.Empty;

    public string SafetyDecisionNarrative { get; set; } = string.Empty;

    public string SafetyDecisionSafe { get; set; } = string.Empty;

    public string SafetyDecisionSafetyPlan { get; set; } = string.Empty;

    public string SafetyDecisionUnsafe { get; set; } = string.Empty;

    public string SafetyDecisionUnsafeChoice { get; set; } = string.Empty;

    public string SafetyDecisionUnsafeChoiceDescription { get; set; } = string.Empty;

    public string SafetyFactor01 { get; set; } = string.Empty;

    public string SafetyFactor01Comment { get; set; } = string.Empty;

    [JsonPropertyName("Safety Factor 01A")]
    public string SafetyFactor01A { get; set; } = string.Empty;

    [JsonPropertyName("Safety Factor 01B")]
    public string SafetyFactor01B { get; set; } = string.Empty;

    [JsonPropertyName("Safety Factor 01C")]
    public string SafetyFactor01C { get; set; } = string.Empty;

    [JsonPropertyName("Safety Factor 01D")]
    public string SafetyFactor01D { get; set; } = string.Empty;

    [JsonPropertyName("Safety Factor 01E")]
    public string SafetyFactor01E { get; set; } = string.Empty;

    public string SafetyFactor02 { get; set; } = string.Empty;

    public string SafetyFactor02Comment { get; set; } = string.Empty;

    public string SafetyFactor03 { get; set; } = string.Empty;

    public string SafetyFactor03Comment { get; set; } = string.Empty;

    public string SafetyFactor04 { get; set; } = string.Empty;

    public string SafetyFactor04Comment { get; set; } = string.Empty;

    public string SafetyFactor05 { get; set; } = string.Empty;

    public string SafetyFactor05Comment { get; set; } = string.Empty;

    public string SafetyFactor06 { get; set; } = string.Empty;

    public string SafetyFactor06Comment { get; set; } = string.Empty;

    public string SafetyFactor07 { get; set; } = string.Empty;

    public string SafetyFactor07Comment { get; set; } = string.Empty;

    public string SafetyFactor08 { get; set; } = string.Empty;

    public string SafetyFactor08Comment { get; set; } = string.Empty;

    public string SafetyFactor09 { get; set; } = string.Empty;

    public string SafetyFactor09Comment { get; set; } = string.Empty;

    public string SafetyFactor10 { get; set; } = string.Empty;

    public string SafetyFactor10Comment { get; set; } = string.Empty;

    public string SafetyFactor11 { get; set; } = string.Empty;

    public string SafetyFactor11Comment { get; set; } = string.Empty;

    public string SafetyFactor12 { get; set; } = string.Empty;

    public string SafetyFactor12Comment { get; set; } = string.Empty;

    public string SafetyFactor13 { get; set; } = string.Empty;

    public string SafetyFactor13Comment { get; set; } = string.Empty;

    public string SafetyFactor14 { get; set; } = string.Empty;

    public string SafetyFactor14Comment { get; set; } = string.Empty;

    public string SafetyIntervention01 { get; set; } = string.Empty;

    public string SafetyIntervention02 { get; set; } = string.Empty;

    public string SafetyIntervention03 { get; set; } = string.Empty;

    public string SafetyIntervention04 { get; set; } = string.Empty;

    public string SafetyIntervention05 { get; set; } = string.Empty;

    public string SafetyIntervention06 { get; set; } = string.Empty;

    public string SafetyIntervention07 { get; set; } = string.Empty;

    public string SafetyIntervention08 { get; set; } = string.Empty;

    public string SafetyIntervention08Other { get; set; } = string.Empty;

    public string SafetyIntervention09 { get; set; } = string.Empty;

    public string SafetyIntervention10 { get; set; } = string.Empty;

    public string SocialWorkerFirstName { get; set; } = string.Empty;

    public string SocialWorkerId { get; set; } = string.Empty;

    public string SocialWorkerLastName { get; set; } = string.Empty;

    public string TeamLeaderFirstName { get; set; } = string.Empty;

    public string TeamLeaderId { get; set; } = string.Empty;

    public string TeamLeaderLastName { get; set; } = string.Empty;

    public string TeamLeaderLoginName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}
