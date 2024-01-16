using Realms;
using System.Globalization;
using Visitz.Extensions;
using VisitzApi.Models.SafetyAssess;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyDecisions : IRealmObject
{
    public static readonly string AllChildrenPlaced = "All children placed";
    public static readonly string SomeChildrenPlaced = "Some children placed";

    private bool NoSafetyFactors { get; set; }
    private bool Safe 
    {
        get => NoSafetyFactors;
        set
        {
            NoSafetyFactors = value;
            SafeInterventions = UnsafeSafetyFactors = false;
        }
    }

    private bool SafeInterventions { get; set; }
    private bool SafeWithInterventions
    {
        get => SafeInterventions;
        set
        {
            SafeInterventions = value;
            NoSafetyFactors = UnsafeSafetyFactors = false;
        }
    }

    private bool UnsafeSafetyFactors { get; set; }
    private bool Unsafe
    {
        get => UnsafeSafetyFactors;
        set
        {
            UnsafeSafetyFactors = value;
            NoSafetyFactors = SafeInterventions = false;
        }
    }

    public SafetyDecisionOption? Decision
    {
        get
        {
            if (Safe)
                return SafetyDecisionOption.Safe;
            else if (SafeWithInterventions)
                return SafetyDecisionOption.SafeWithInterventions;
            else if (Unsafe)
                return SafetyDecisionOption.Unsafe;
            else
                return null;
        }
        set
        {
            if (IsManaged)
                Realm.Write(() => SetDecision(value));
            else
                SetDecision(value);
        }
    }

    private void SetDecision(SafetyDecisionOption? option)
    {
        if (option == null)
            NoSafetyFactors = SafeInterventions = UnsafeSafetyFactors = false;
        else if (option.Equals(SafetyDecisionOption.Safe))
            Safe = true;
        else if (option.Equals(SafetyDecisionOption.SafeWithInterventions))
            SafeWithInterventions = true;
        else if (option.Equals(SafetyDecisionOption.Unsafe))
            Unsafe = true;
    }

    public string DecisionUnsafe { get; set; } = string.Empty; // Max length 255

    public string Comments { get; set; } = string.Empty; // Max length 8000

    public string Narrative { get; set; }  = string.Empty; // Max length 2000

    public bool ReadyFinalize { get; set; }

    public DateTimeOffset ReadyFinalizeDate { get; set; } = DateTimeOffset.Now; // Only date, no time

    public static SafetyDecisions FromApiEntity(SafetyDecisionsEntity entity)
    {
        return new SafetyDecisions()
        {
            NoSafetyFactors = entity.NoSafetyFactors.ParseWordTruthiness(),
            SafeInterventions = entity.SafeInterventions.ParseWordTruthiness(),
            UnsafeSafetyFactors = entity.UnsafeSafetyFactors.ParseWordTruthiness(),
            DecisionUnsafe = entity.DecisionUnsafe,
            Comments = entity.Comments,
            Narrative = entity.Narrative,
            ReadyFinalize = entity.ReadyFinalize.ParseWordTruthiness(),
            ReadyFinalizeDate = DateTimeOffset.Parse(entity.ReadyFinalizeDate),
        };
    }

    public SafetyDecisionsEntity ToApiEntity()
    {
        var finalizeDate = ReadyFinalize
            ? ReadyFinalizeDate.ToString(SafetyAssessment.DateFormat, CultureInfo.InvariantCulture)
            : "";

        return new SafetyDecisionsEntity()
        {
            NoSafetyFactors = NoSafetyFactors.AsTruthyChar(),
            SafeInterventions = SafeInterventions.AsTruthyChar(),
            UnsafeSafetyFactors = UnsafeSafetyFactors.AsTruthyChar(),
            DecisionUnsafe = DecisionUnsafe,
            Comments = Comments,
            Narrative = Narrative,
            ReadyFinalize = ReadyFinalize.AsTruthyChar(),
            ReadyFinalizeDate = finalizeDate,
        };
    }
}
