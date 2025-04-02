using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.SafetyAssess;

public partial class FactorInfluence : IRealmObject, IApiJson<SubmitFactorInfluenceJson>
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

	public SubmitFactorInfluenceJson ToApiJson(string _ = "s")
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
