/*
	Partial class implementation of a Realm + compiled bindings workaround.

	https://github.com/realm/realm-dotnet/issues/2270#issuecomment-786720318
 */

using Visitz.Extensions;

namespace Visitz.Models.SafetyAssess;

public partial class ProtectiveCapacity
{
	public bool ChildCognitiveBinding
    {
        get => IsValid ? ChildCognitive : default;
        set => this.Commit(() => ChildCognitive = value);
    }
	
    public bool ParentCognitiveBinding
    {
        get => IsValid ? ParentCognitive : default;
        set => this.Commit(() => ParentCognitive = value);
    }
    
    public bool ParentWillingnessBinding
    {
        get => IsValid ? ParentWillingness : default;
        set => this.Commit(() => ParentWillingness = value);
    }
    
    public bool ParentResourcesBinding
    {
        get => IsValid ? ParentResources : default;
        set => this.Commit(() => ParentResources = value);
    }
    
    public bool ParentSupportiveBinding
    {
        get => IsValid ? ParentSupportive : default;
        set => this.Commit(() => ParentSupportive = value);
    }
    
    public bool ParentProtectBinding
    {
        get => IsValid ? ParentProtect : default;
        set => this.Commit(() => ParentProtect = value);
    }
    
    public bool ParentAcceptBinding
    {
        get => IsValid ? ParentAccept : default;
        set => this.Commit(() => ParentAccept = value);
    }
    
    public bool ParentRelationshipBinding
    {
        get => IsValid ? ParentRelationship : default;
        set => this.Commit(() => ParentRelationship = value);
    }
    
    public bool ParentAwareBinding
    {
        get => IsValid ? ParentAware : default;
        set => this.Commit(() => ParentAware = value);
    }
    
    public bool ParentProbSolvingBinding
    {
        get => IsValid ? ParentProbSolving : default;
        set => this.Commit(() => ParentProbSolving = value);
    }
    
    public bool NoProCapPresentBinding
    {
        get => IsValid ? NoProCapPresent : default;
        set => this.Commit(() => NoProCapPresent = value);
    }
    
    public bool CapacitiesOtherBinding
    {
        get => IsValid ? CapacitiesOther : default;
        set => this.Commit(() => CapacitiesOther = value);
    }

    public string CmtProtectiveCapacity01Binding
    {
        get => IsValid ? CmtProtectiveCapacity01 : default;
        set => this.Commit(() => CmtProtectiveCapacity01 = value);
    }
    
    public string CmtProtectiveCapacity02Binding
    {
        get => IsValid ? CmtProtectiveCapacity02 : default;
        set => this.Commit(() => CmtProtectiveCapacity02 = value);
    }
}
