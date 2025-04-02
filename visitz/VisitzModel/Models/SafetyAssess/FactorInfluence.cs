using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

public partial class FactorInfluence : IRealmObject
{
	public bool AgeUptoFive { get; set; }
	
	public bool MedicalMentalDisorder { get; set; }
	
	public bool NotReadilyAccessible { get; set; }
	
	public bool DiminishedMental { get; set; }
	
	public bool DiminishedPhysical { get; set; }

    public static FactorInfluence FromApiEntity(SubmitFactorInfluenceJson entity)
	{
		return new FactorInfluence()
		{
			AgeUptoFive = entity.AgeUptoFive.ParseWordTruthiness(),
			MedicalMentalDisorder = entity.MedicalMentalDisorder.ParseWordTruthiness(),
			NotReadilyAccessible = entity.NotReadilyAccessible.ParseWordTruthiness(),
			DiminishedMental = entity.DiminishedMental.ParseWordTruthiness(),
			DiminishedPhysical = entity.DiminishedPhysical.ParseWordTruthiness(),
		};
	}

	public SubmitFactorInfluenceJson ToApiEntity()
	{
		return new SubmitFactorInfluenceJson()
		{
            AgeUptoFive = AgeUptoFive.AsTruthyChar(),
            MedicalMentalDisorder = MedicalMentalDisorder.AsTruthyChar(),
            NotReadilyAccessible = NotReadilyAccessible.AsTruthyChar(),
            DiminishedMental = DiminishedMental.AsTruthyChar(),
            DiminishedPhysical = DiminishedPhysical.AsTruthyChar(),
        };
	}
}
