/*
    Partial class implementation of a Realm + compiled bindings workaround.

    https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyDecisions
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string? propertyName)
    {
        if (propertyName != null && !propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public SafetyDecisionOption? DecisionBinding
    {
        get => IsValid ? Decision : default;
        set
        {
            this.Commit(() => Decision = value);

            if (value != SafetyDecisionOption.Unsafe)
                DecisionUnsafeBinding = null;

            RaisePropertyChanged(nameof(IsAnswered));
        }
    }

    public string? DecisionUnsafeBinding
    {
        get => IsValid ? DecisionUnsafe : default;
        set
        {
            this.Commit(() => DecisionUnsafe = value);
            RaisePropertyChanged(nameof(IsAnswered));
        }
    }

    public string? CommentsBinding
    {
        get => IsValid ? Comments : default;
        set => this.Commit(() => Comments = value);
    }

    public string? NarrativeBinding
    {
        get => IsValid ? Narrative : default;
        set => this.Commit(() => Narrative = value);
    }

    public bool ReadyFinalizeBinding
    {
        get => IsValid && ReadyFinalize;
        set => this.Commit(() => ReadyFinalize = value);
    }

    public DateTimeOffset? ReadyFinalizeDateBinding
    {
        get => IsValid ? ReadyFinalizeDate : default;
        set => this.Commit(() => ReadyFinalizeDate = value);
    }
}
