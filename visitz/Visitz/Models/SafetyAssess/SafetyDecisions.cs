using Realms;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyDecisions : IRealmObject
{
    public bool NoSafetyFactors { get; set; }

    public bool SafeInterventions { get; set; }

    public bool UnsafeSafetyFactors { get; set; }

    public string DecisionUnsafe { get; set; }

    public string Comments { get; set; }

    public string Narrative { get; set; }

    public bool ReadyFinalize { get; set; }

    public DateTimeOffset ReadyFinalizeDate { get; set; } // Only date, no time

}
