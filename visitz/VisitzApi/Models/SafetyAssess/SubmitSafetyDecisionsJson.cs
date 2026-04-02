using System.Text.Json.Serialization;

namespace VisitzApi.Models.SafetyAssess;

public class SubmitSafetyDecisionsJson
{
    public string NoSafetyFactors { get; set; } = string.Empty;

    public string SafeInterventions { get; set; } = string.Empty;

    public string UnsafeSafetyFactors { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DecisionUnsafe { get; set; }

    public string Comments { get; set; } = string.Empty;

    public string Narrative { get; set; } = string.Empty;

    public string ReadyFinalize { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReadyFinalizeDate { get; set; }
}
