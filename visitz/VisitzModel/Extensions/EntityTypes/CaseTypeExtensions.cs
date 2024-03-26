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

	public static CaseType ParseCaseType(this string str)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AdoptionHome))
			return CaseType.AdoptionHome;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AdoptionService))
			return CaseType.AdoptionService;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AssessmentOnly))
			return CaseType.AssessmentOnly;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.AssistedAdoption))
			return CaseType.AssistedAdoption;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.Autism))
			return CaseType.Autism;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.CysnFamilyServices))
			return CaseType.CysnFamilyServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.ChildAtHomeProgram))
			return CaseType.ChildAtHomeProgram;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.ChildServices))
			return CaseType.ChildServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.FamilyServices))
			return CaseType.FamilyServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.MedicalBenefits))
			return CaseType.MedicalBenefits;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.PostAdoptionService))
			return CaseType.PostAdoptionService;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.PostMajorityServices))
			return CaseType.PostMajorityServices;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.Resource))
			return CaseType.Resource;
		else if (EntityTypeExtensions.Matches(str, CaseTypeStrings.SupportNeedsChildrenYouth))
			return CaseType.SupportNeedsChildrenYouth;
		else
			return CaseType.Unknown;
	}

	public static bool TryParseCaseType(this string str, out CaseType caseType)
	{
		caseType = ParseCaseType(str);
		return caseType > CaseType.Unknown && caseType <= CaseType.SupportNeedsChildrenYouth;
	}
}
