/*
	Partial class implementation of a Realm + compiled bindings workaround.

	https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using Visitz.Extensions;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyDecisions
{
    public SafetyDecisionOption? DecisionBinding
    {
        get => IsValid ? Decision : default;
        set => this.Commit(() => Decision = value);
    }

    public string DecisionUnsafeBinding
    {
        get => IsValid ? DecisionUnsafe : default;
        set => this.Commit(() => DecisionUnsafe = value);
    }

    public string CommentsBinding
    {
        get => IsValid ? Comments : default;
        set => this.Commit(() => Comments = value);
    }

    public string NarrativeBinding
    {
        get => IsValid ? Narrative : default;
        set => this.Commit(() => Narrative = value);
    }

    public bool ReadyFinalizeBinding
    {
        get => IsValid ? ReadyFinalize : default;
        set => this.Commit(() => ReadyFinalize = value);
    }

    public DateTimeOffset ReadyFinalizeDateBinding
    {
        get => IsValid ? ReadyFinalizeDate : default;
        set => this.Commit(() => ReadyFinalizeDate = value);
    }
}
