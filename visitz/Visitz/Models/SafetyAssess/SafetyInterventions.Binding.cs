/*
	Partial class implementation of a Realm + compiled bindings workaround.

	https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using Visitz.Extensions;

namespace Visitz.Models.SafetyAssess;

public partial class SafetyInterventions
{
    private const string Binding = "Binding";

    partial void OnPropertyChanged(string propertyName)
    {
        if (!propertyName.EndsWith(Binding))
            RaisePropertyChanged($"{propertyName}{Binding}");
    }

    public bool DirectInterventionBinding
    {
        get => IsValid ? DirectIntervention : default;
        set => this.Commit(() => DirectIntervention = value);
    }
    
    public bool UseOfIndividualsBinding
    {
        get => IsValid ? UseOfIndividuals : default;
        set => this.Commit(() => UseOfIndividuals = value);
    }
    
    public bool UseCommAgenciesBinding
    {
        get => IsValid ? UseCommAgencies : default;
        set => this.Commit(() => UseCommAgencies = value);
    }
    
    public bool ProtectVictimBinding
    {
        get => IsValid ? ProtectVictim : default;
        set => this.Commit(() => ProtectVictim = value);
    }
    
    public bool LeaveHomeBinding
    {
        get => IsValid ? LeaveHome : default;
        set => this.Commit(() => LeaveHome = value);
    }
    
    public bool NonOffendingParentBinding
    {
        get => IsValid ? NonOffendingParent : default;
        set => this.Commit(() => NonOffendingParent = value);
    }
    
    public bool LegalIntPlannedBinding
    {
        get => IsValid ? LegalIntPlanned : default;
        set => this.Commit(() => LegalIntPlanned = value);
    }
    
    public bool OtherSafetyInterventionsBinding
    {
        get => IsValid ? OtherSafetyInterventions : default;
        set => this.Commit(() => OtherSafetyInterventions = value);
    }

    public string CmtSafetyInterventionsBinding
    {
        get => IsValid ? CmtSafetyInterventions : default;
        set => this.Commit(() => CmtSafetyInterventions = value);
    }
    
    public bool ChildOutsideHomeBinding
    {
        get => IsValid ? ChildOutsideHome : default;
        set => this.Commit(() => ChildOutsideHome = value);
    }
    
    public bool ChildRemovedBinding
    {
        get => IsValid ? ChildRemoved : default;
        set => this.Commit(() => ChildRemoved = value);
    }
}
