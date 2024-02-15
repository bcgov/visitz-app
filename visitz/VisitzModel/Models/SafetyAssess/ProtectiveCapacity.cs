using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

public partial class ProtectiveCapacity : IRealmObject
{
	public bool ChildCognitive { get; set; }
	
    public bool ParentCognitive { get; set; }
    
    public bool ParentWillingness { get; set; }
    
    public bool ParentResources { get; set; }
    
    public bool ParentSupportive { get; set; }
    
    public bool ParentProtect { get; set; }
    
    public bool ParentAccept { get; set; }
    
    public bool ParentRelationship { get; set; }
    
    public bool ParentAware { get; set; }
    
    public bool ParentProbSolving { get; set; }
    
    public bool NoProCapPresent { get; set; }
    
    public bool CapacitiesOther { get; set; }

    public string CmtProtectiveCapacity01 { get; set; } = string.Empty;
    
    public string CmtProtectiveCapacity02 { get; set; } = string.Empty;

    public static ProtectiveCapacity FromApiEntity(ProtectiveCapacityEntity entity)
    {
        return new ProtectiveCapacity()
        {
            ChildCognitive = entity.ChildCognitive.ParseWordTruthiness(),
            ParentCognitive = entity.ParentCognitive.ParseWordTruthiness(),
            ParentWillingness = entity.ParentWillingness.ParseWordTruthiness(),
            ParentResources = entity.ParentResources.ParseWordTruthiness(),
            ParentSupportive = entity.ParentSupportive.ParseWordTruthiness(),
            ParentProtect = entity.ParentProtect.ParseWordTruthiness(),
            ParentAccept = entity.ParentAccept.ParseWordTruthiness(),
            ParentRelationship = entity.ParentRelationship.ParseWordTruthiness(),
            ParentAware = entity.ParentAware.ParseWordTruthiness(),
            ParentProbSolving = entity.ParentProbSolving.ParseWordTruthiness(),
            NoProCapPresent = entity.NoProCapPresent.ParseWordTruthiness(),
            CapacitiesOther = entity.CapacitiesOther.ParseWordTruthiness(),
            CmtProtectiveCapacity01 = entity.CmtProtectiveCapacity01,
            CmtProtectiveCapacity02 = entity.CmtProtectiveCapacity02,
        };
    }

    public ProtectiveCapacityEntity ToApiEntity()
    {
        return new ProtectiveCapacityEntity()
        {
            ChildCognitive = ChildCognitive.AsTruthyChar(),
            ParentCognitive = ParentCognitive.AsTruthyChar(),
            ParentWillingness = ParentWillingness.AsTruthyChar(),
            ParentResources = ParentResources.AsTruthyChar(),
            ParentSupportive = ParentSupportive.AsTruthyChar(),
            ParentProtect = ParentProtect.AsTruthyChar(),
            ParentAccept = ParentAccept.AsTruthyChar(),
            ParentRelationship = ParentRelationship.AsTruthyChar(),
            ParentAware = ParentAware.AsTruthyChar(),
            ParentProbSolving = ParentProbSolving.AsTruthyChar(),
            NoProCapPresent = NoProCapPresent.AsTruthyChar(),
            CapacitiesOther = CapacitiesOther.AsTruthyChar(),
            CmtProtectiveCapacity01 = CapacitiesOther ? CmtProtectiveCapacity01 : "",
            CmtProtectiveCapacity02 = CmtProtectiveCapacity02,
        };
    }

    /// <summary>
    /// Used for form business logic: "Protective Capacities section - Checking 'No protective capacities present' 
    /// unchecks all other Protective Capacities checkboxes. Checking any other checkbox unchecks "No protective 
    /// capacities present" checkbox".
    /// </summary>
    private void ClearNoProCapPresent()
    {
        NoProCapPresentBinding = false;
    }

    /// <summary>
    /// Used for form business logic: "Protective Capacities section - Checking 'No protective capacities present' 
    /// unchecks all other Protective Capacities checkboxes. Checking any other checkbox unchecks "No protective 
    /// capacities present" checkbox".
    /// </summary>
    /// <param name="newVal">Assigned directly to NoProCapPresent</param>
    private void SetNoProCapPresent(bool newVal)
    {
        NoProCapPresent = newVal;

        if (NoProCapPresent)
        {
            ParentCognitive =
            ParentWillingness =
            ParentResources =
            ParentSupportive =
            ParentProtect =
            ParentAccept =
            ParentRelationship =
            ParentAware =
            ParentProbSolving =
            CapacitiesOther = false;
        }
    }
}
