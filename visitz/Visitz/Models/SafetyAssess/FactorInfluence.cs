using Realms;
using Visitz.Extensions;
using VisitzModel.Extensions;
using VisitzApi.Models.SafetyAssess;

namespace Visitz.Models.SafetyAssess;

public partial class FactorInfluence : IRealmObject
{
	public bool AgeUptoFive { get; set; }
	
	public bool MedicalMentalDisorder { get; set; }
	
	public bool NotReadilyAccessible { get; set; }
	
	public bool DiminishedMental { get; set; }
	
	public bool DiminishedPhysical { get; set; }

    public static FactorInfluence FromApiEntity(FactorInfluenceEntity entity)
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

	public FactorInfluenceEntity ToApiEntity()
	{
		return new FactorInfluenceEntity()
		{
            AgeUptoFive = AgeUptoFive.AsTruthyChar(),
            MedicalMentalDisorder = MedicalMentalDisorder.AsTruthyChar(),
            NotReadilyAccessible = NotReadilyAccessible.AsTruthyChar(),
            DiminishedMental = DiminishedMental.AsTruthyChar(),
            DiminishedPhysical = DiminishedPhysical.AsTruthyChar(),
        };
	}
}
