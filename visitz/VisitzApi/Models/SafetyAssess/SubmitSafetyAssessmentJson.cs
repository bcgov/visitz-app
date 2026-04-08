using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisitzApi.Models.SafetyAssess;

public class SubmitSafetyAssessmentJson
{
    [Required]
    [JsonPropertyName("Payload")]
    public List<SubmitSafetyAssessmentHeaderJson> Payload { get; set; } = [];

    [Required]
    public List<SubmitFactorInfluenceJson> FactorInfluence { get; set; } = [];

    [Required]
    public List<SubmitSafetyFactorsJson> SafetyFactors { get; set; } = [];

    [Required]
    public List<SubmitProtectiveCapacityJson> ProtectiveCapacity { get; set; } = [];

    [Required]
    public List<SubmitSafetyInterventionsJson> SafetyInterventions { get; set; } = [];

    [Required]
    public List<SubmitSafetyDecisionsJson> SafetyDecisions { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<ChildId>? ChildsInOutCare { get; set; }

    public class ChildId
    {
        public string ChildContactId { get; set; } = string.Empty;

        public override string ToString() => ChildContactId;
    }

    public void AddChildContactId(string childContactid)
    {
        ChildsInOutCare ??= [];
        ChildsInOutCare.Add(new ChildId { ChildContactId = childContactid });
    }
}
