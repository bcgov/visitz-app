using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class CaseTypeExtensions
{
	public static string ToString(this CaseType caseType)
	{
		return caseType switch
		{
			CaseType.AdoptionHome => CaseTypeStrings.AdoptionHome,
			CaseType.AdoptionService => CaseTypeStrings.AdoptionService,
			CaseType.AssessmentOnly => CaseTypeStrings.AssessmentOnly,
			CaseType.AssistedAdoption => CaseTypeStrings.AssistedAdoption,
			CaseType.Autism => CaseTypeStrings.Autism,
			CaseType.CysnFamilyServices => CaseTypeStrings.CysnFamilyServices,
			CaseType.ChildAtHomeProgram => CaseTypeStrings.ChildAtHomeProgram,
			CaseType.ChildServices => CaseTypeStrings.ChildServices,
			CaseType.FamilyServices => CaseTypeStrings.FamilyServices,
			CaseType.MedicalBenefits => CaseTypeStrings.MedicalBenefits,
			CaseType.PostAdoptionService => CaseTypeStrings.PostAdoptionService,
			CaseType.PostMajorityServices => CaseTypeStrings.PostMajorityServices,
			CaseType.Resource => CaseTypeStrings.Resource,
			CaseType.SupportNeedsChildrenYouth => CaseTypeStrings.SupportNeedsChildrenYouth,
			_ => throw new NotImplementedException(),
		};
	}

	public static bool TryParseCaseType(this string str, out CaseType caseType)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AdoptionHome))
			caseType = CaseType.AdoptionHome;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AdoptionService))
			caseType = CaseType.AdoptionService;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AssessmentOnly))
			caseType = CaseType.AssessmentOnly;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AssistedAdoption))
			caseType = CaseType.AssistedAdoption;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.Autism))
			caseType = CaseType.Autism;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.CysnFamilyServices))
			caseType = CaseType.CysnFamilyServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.ChildAtHomeProgram))
			caseType = CaseType.ChildAtHomeProgram;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.ChildServices))
			caseType = CaseType.ChildServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.FamilyServices))
			caseType = CaseType.FamilyServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.MedicalBenefits))
			caseType = CaseType.MedicalBenefits;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.PostAdoptionService))
			caseType = CaseType.PostAdoptionService;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.PostMajorityServices))
			caseType = CaseType.PostMajorityServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.Resource))
			caseType = CaseType.Resource;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.SupportNeedsChildrenYouth))
			caseType = CaseType.SupportNeedsChildrenYouth;
		else
			caseType = CaseType.Unknown;

		return caseType > CaseType.Unknown && caseType <= CaseType.SupportNeedsChildrenYouth;
	}
}
