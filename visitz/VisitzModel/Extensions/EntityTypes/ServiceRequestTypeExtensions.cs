using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class ServiceRequestTypeExtensions
{
	public static string ToString(this ServiceRequestType serviceRequestType)
	{
		return serviceRequestType switch
		{
			ServiceRequestType.AfterHours => ServiceRequestTypeStrings.AfterHours,
			ServiceRequestType.AutismFundingUnit => ServiceRequestTypeStrings.AutismFundingUnit,
			ServiceRequestType.CommunityDevelopment => ServiceRequestTypeStrings.CommunityDevelopment,
			ServiceRequestType.FamilyNeedsAssessment => ServiceRequestTypeStrings.FamilyNeedsAssessment,
			ServiceRequestType.MedicalBenefits => ServiceRequestTypeStrings.MedicalBenefits,
			ServiceRequestType.PostMajorityServices => ServiceRequestTypeStrings.PostMajorityServices,
			ServiceRequestType.RequestServiceCfs => ServiceRequestTypeStrings.RequestServiceCfs,
			ServiceRequestType.RequestServiceCapp => ServiceRequestTypeStrings.RequestServiceCapp,
			ServiceRequestType.RequestForFamilySupport => ServiceRequestTypeStrings.RequestForFamilySupport,
			ServiceRequestType.RequestForInformation => ServiceRequestTypeStrings.RequestForInformation,
			ServiceRequestType.YouthServices => ServiceRequestTypeStrings.YouthServices,
			_ => throw new NotImplementedException(),
		};
	}

	public static ServiceRequestType ParseServiceRequestType(this string str)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.AfterHours))
			return ServiceRequestType.AfterHours;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.AutismFundingUnit))
			return ServiceRequestType.AutismFundingUnit;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.CommunityDevelopment))
			return ServiceRequestType.CommunityDevelopment;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.FamilyNeedsAssessment))
			return ServiceRequestType.FamilyNeedsAssessment;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.MedicalBenefits))
			return ServiceRequestType.MedicalBenefits;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.PostMajorityServices))
			return ServiceRequestType.PostMajorityServices;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestServiceCfs))
			return ServiceRequestType.RequestServiceCfs;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestServiceCapp))
			return ServiceRequestType.RequestServiceCapp;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestForFamilySupport))
			return ServiceRequestType.RequestForFamilySupport;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestForInformation))
			return ServiceRequestType.RequestForInformation;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.YouthServices))
			return ServiceRequestType.YouthServices;
		else
			return ServiceRequestType.Unknown;
	}

	public static bool TryParseServiceRequestType(this string str, out ServiceRequestType serviceRequestType)
	{
		serviceRequestType = ParseServiceRequestType(str);

		return serviceRequestType > ServiceRequestType.Unknown
			&& serviceRequestType <= ServiceRequestType.YouthServices;
	}
}
