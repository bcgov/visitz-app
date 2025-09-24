namespace VisitzApi.Models.SafetyAssess;

#nullable enable

public class SubmitSafetyDecisionsJson
{
    public string NoSafetyFactors { get; set; } = string.Empty;

    public string SafeInterventions { get; set; } = string.Empty;

    public string UnsafeSafetyFactors { get; set; } = string.Empty;

    public string DecisionUnsafe { get; set; } = string.Empty;

    public string Comments { get; set; } = string.Empty;

    public string Narrative { get; set; } = string.Empty;

    public string ReadyFinalize { get; set; } = string.Empty;

    public string ReadyFinalizeDate { get; set; } = string.Empty;
}
