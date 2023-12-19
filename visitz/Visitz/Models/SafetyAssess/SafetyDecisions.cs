using Realms;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyDecisions : IRealmObject
{
    public static readonly string AllChildrenPlaced = "All children placed";
    public static readonly string SomeChildrenPlaced = "Some children placed";

    public bool NoSafetyFactors { get; set; }

    public bool SafeInterventions { get; set; }

    public bool UnsafeSafetyFactors { get; set; }

    public string DecisionUnsafe { get; set; } // Max length 255

    public string Comments { get; set; } // Max length 8000

    public string Narrative { get; set; } // Max length 2000

    public bool ReadyFinalize { get; set; }

    public DateTimeOffset ReadyFinalizeDate { get; set; } // Only date, no time

}
