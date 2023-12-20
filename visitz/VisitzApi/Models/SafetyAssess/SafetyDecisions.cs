namespace VisitzApi.Models.SafetyAssess;

public class SafetyDecisionsEntity
{
    public string NoSafetyFactors { get; set; }

    public string SafeInterventions { get; set; }

    public string UnsafeSafetyFactors { get; set; }

    public string DecisionUnsafe { get; set; }

    public string Comments { get; set; }

    public string Narrative { get; set; }

    public string ReadyFinalize { get; set; }

    public string ReadyFinalizeDate { get; set; }
}
