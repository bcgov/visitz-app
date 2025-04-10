/*
	Partial class implementation of a Realm + compiled bindings workaround.

	https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyAssessment
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string propertyName)
    {
        if (!propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public string IncidentNumberBinding
    {
        get => IsValid ? IncidentNumber : default;
        set => this.Commit(() => IncidentNumber = value);
    }

    public string WorkerIdBinding
    {
        get => IsValid ? WorkerId : default;
        set => this.Commit(() => WorkerId = value);
    }

    public string FamilyNameBinding
    {
        get => IsValid ? FamilyName : default;
        set => this.Commit(() => FamilyName = value);
    }

    public DateTimeOffset? DateOfAssessmentBinding
    {
        get => IsValid ? DateOfAssessment : default;
        set => this.Commit(() => DateOfAssessment = value);
    }

    public string OperationBinding
    {
        get => IsValid ? Operation : default;
        set => this.Commit(() => Operation = value);
    }
}
