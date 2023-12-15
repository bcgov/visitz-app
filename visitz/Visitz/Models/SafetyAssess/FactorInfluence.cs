using Realms;

namespace Visitz.Models.SafetyAssess;

public partial class FactorInfluence : IRealmObject
{
	public bool AgeUptoFive { get; set; }
	
	public bool MedicalMentalDisorder { get; set; }
	
	public bool NotReadilyAccessible { get; set; }
	
	public bool DiminishedMental { get; set; }
	
	public bool DiminishedPhysical { get; set; }
	
}
