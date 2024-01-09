using Realms;
using System.Globalization;
using Visitz.Extensions;
using VisitzApi.Models.SafetyAssess;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyDecisions : IRealmObject
{
    public static readonly string AllChildrenPlaced = "All children placed";
    public static readonly string SomeChildrenPlaced = "Some children placed";

    public bool NoSafetyFactors { get; set; }

    public bool SafeInterventions { get; set; }

    public bool UnsafeSafetyFactors { get; set; }

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
