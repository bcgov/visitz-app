using System.Globalization;
using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Utilities;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyDecisions : IRealmObject, IApiJson<SubmitSafetyDecisionsJson>
{
    public static readonly string AllChildrenPlaced = "All children placed";
    public static readonly string SomeChildrenPlaced = "Some children placed";

    public static readonly int CommentsMaxLength = 16000;
    public static readonly int NarrativeMaxLength = 2000;

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
        set => this.Commit(() => SetDecision(value));
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

    public string? DecisionUnsafe { get; set; } // Max length 255

    public string? DecisionUnsafeDescription { get; set; }

    public string? Comments { get; set; } = string.Empty; // Max length 8000

    public string? Narrative { get; set; } = string.Empty; // Max length 2000

    public bool ReadyFinalize { get; set; }

    public DateTimeOffset? ReadyFinalizeDate { get; set; } // Only date, no time

    public bool IsAnswered =>
        Decision != null && (Decision != SafetyDecisionOption.Unsafe || DecisionUnsafe?.Length > 0);

    public static SafetyDecisions FromApiJson(GetSafetyAsessmentJson entity)
    {
        return new SafetyDecisions()
        {
            NoSafetyFactors = entity.SafetyDecisionSafe.ParseWordTruthiness(),
            SafeInterventions = entity.SafetyDecisionIntervention.ParseWordTruthiness(),
            UnsafeSafetyFactors = entity.SafetyDecisionUnsafe.ParseWordTruthiness(),
            DecisionUnsafe = entity.SafetyDecisionUnsafeChoice,
            DecisionUnsafeDescription = entity.SafetyDecisionUnsafeChoiceDescription,
            Comments = entity.SafetyDecisionSafetyPlan,
            Narrative = entity.SafetyDecisionNarrative,
            ReadyFinalize = entity.ReadyToFinalize.ParseWordTruthiness(),
            ReadyFinalizeDate = Timestamp.ParseDateTimeOffsetNullable(entity.ReadyToFinalizeDate),
        };
    }

    public SubmitSafetyDecisionsJson ToApiJson(string _ = "s")
    {
        string? finalizeDate = ReadyFinalize
            ? DateTimeOffset.Now.ToString(SafetyAssessment.DateFormat, CultureInfo.InvariantCulture)
            : null;

        return new SubmitSafetyDecisionsJson()
        {
            NoSafetyFactors = NoSafetyFactors.AsTruthyChar(),
            SafeInterventions = SafeInterventions.AsTruthyChar(),
            UnsafeSafetyFactors = UnsafeSafetyFactors.AsTruthyChar(),
            DecisionUnsafe = IsAnswered ? DecisionUnsafe : null,
            Comments = SafeInterventions ? Comments ?? string.Empty : string.Empty,
            Narrative = Narrative ?? string.Empty,
            ReadyFinalize = ReadyFinalize.AsTruthyChar(),
            ReadyFinalizeDate = finalizeDate,
        };
    }
}
