using System.Text.Json.Serialization;
using VisitzApi.Models.Base;

namespace VisitzApi.Models.SafetyAssess;

public class SafetyAsessmentJson : BaseRecordJson
{
    public class ChildContacts
    {
        public string Id { get; set; }
    }

    public string ApprovedBy { get; set; }

    public string ApprovedDate { get; set; }

    public string ApprovedToFinalize { get; set; }

    public string ApprovedToFinalizeDate { get; set; }

    public string ApprovedToFinalizeDS { get; set; }

    public string ChildContactId { get; set; }

    public IList<ChildContacts> ContactsInOutCare { get; set; }

    public string DataStewardRole { get; set; }

    public string DateOfAssessment { get; set; }

    public string FactorInfluence1 { get; set; }

    public string FactorInfluence2 { get; set; }

    public string FactorInfluence3 { get; set; }

    public string FactorInfluence4 { get; set; }

    public string FactorInfluence5 { get; set; }

    public string FamilyName { get; set; }

    public string FinalizedDate { get; set; }

    public string ProtectiveCapacity01 { get; set; }

    public string ProtectiveCapacity02 { get; set; }

    public string ProtectiveCapacity03 { get; set; }

    public string ProtectiveCapacity04 { get; set; }

    public string ProtectiveCapacity05 { get; set; }

    public string ProtectiveCapacity06 { get; set; }

    public string ProtectiveCapacity07 { get; set; }

    public string ProtectiveCapacity08 { get; set; }

    public string ProtectiveCapacity09 { get; set; }

    public string ProtectiveCapacity10 { get; set; }

    public string ProtectiveCapacity11 { get; set; }

    public string ProtectiveCapacity12 { get; set; }

    public string ProtectiveCapacity12Other { get; set; }

    public string ProtectiveCapacityObservations { get; set; }

    public string ReadyToFinalize { get; set; }

    public string ReadyToFinalizeDate { get; set; }

    public string RowId { get; set; }

    public string SafetyDecisionIntervention { get; set; }

    public string SafetyDecisionNarrative { get; set; }

    public string SafetyDecisionSafe { get; set; }

    public string SafetyDecisionSafetyPlan { get; set; }

    public string SafetyDecisionUnsafe { get; set; }

    public string SafetyDecisionUnsafeChoice { get; set; }

    public string SafetyDecisionUnsafeChoiceDescription { get; set; }

    public string SafetyFactor01 { get; set; }

    public string SafetyFactor01Comment { get; set; }

    [JsonPropertyName("Safety Factor 01A")]
    public string SafetyFactor01A { get; set; }

    [JsonPropertyName("Safety Factor 01B")]
    public string SafetyFactor01B { get; set; }

    [JsonPropertyName("Safety Factor 01C")]
    public string SafetyFactor01C { get; set; }

    [JsonPropertyName("Safety Factor 01D")]
    public string SafetyFactor01D { get; set; }

    [JsonPropertyName("Safety Factor 01E")]
    public string SafetyFactor01E { get; set; }

    public string SafetyFactor02 { get; set; }

    public string SafetyFactor02Comment { get; set; }

    public string SafetyFactor03 { get; set; }

    public string SafetyFactor03Comment { get; set; }

    public string SafetyFactor04 { get; set; }

    public string SafetyFactor04Comment { get; set; }

    public string SafetyFactor05 { get; set; }

    public string SafetyFactor05Comment { get; set; }

    public string SafetyFactor06 { get; set; }

    public string SafetyFactor06Comment { get; set; }

    public string SafetyFactor07 { get; set; }

    public string SafetyFactor07Comment { get; set; }

    public string SafetyFactor08 { get; set; }

    public string SafetyFactor08Comment { get; set; }

    public string SafetyFactor09 { get; set; }

    public string SafetyFactor09Comment { get; set; }

    public string SafetyFactor10 { get; set; }

    public string SafetyFactor10Comment { get; set; }

    public string SafetyFactor11 { get; set; }

    public string SafetyFactor11Comment { get; set; }

    public string SafetyFactor12 { get; set; }

    public string SafetyFactor12Comment { get; set; }

    public string SafetyFactor13 { get; set; }

    public string SafetyFactor13Comment { get; set; }

    public string SafetyFactor14 { get; set; }

    public string SafetyFactor14Comment { get; set; }

    public string SafetyIntervention01 { get; set; }

    public string SafetyIntervention02 { get; set; }

    public string SafetyIntervention03 { get; set; }

    public string SafetyIntervention04 { get; set; }

    public string SafetyIntervention05 { get; set; }

    public string SafetyIntervention06 { get; set; }

    public string SafetyIntervention07 { get; set; }

    public string SafetyIntervention08 { get; set; }

    public string SafetyIntervention08Other { get; set; }

    public string SafetyIntervention09 { get; set; }

    public string SafetyIntervention10 { get; set; }

    public string SocialWorkerFirstName { get; set; }

    public string SocialWorkerId { get; set; }

    public string SocialWorkerLastName { get; set; }

    public string TeamLeaderFirstName { get; set; }

    public string TeamLeaderId { get; set; }

    public string TeamLeaderLastName { get; set; }

    public string TeamLeaderLoginName { get; set; }

    public string Type { get; set; }
}
