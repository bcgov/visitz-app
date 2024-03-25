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

	public static bool TryParseServiceRequestType(this string str, out ServiceRequestType serviceRequestType)
	{
		str = str.Trim();

		if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.AfterHours))
			serviceRequestType = ServiceRequestType.AfterHours;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.AutismFundingUnit))
			serviceRequestType = ServiceRequestType.AutismFundingUnit;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.CommunityDevelopment))
			serviceRequestType = ServiceRequestType.CommunityDevelopment;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.FamilyNeedsAssessment))
			serviceRequestType = ServiceRequestType.FamilyNeedsAssessment;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.MedicalBenefits))
			serviceRequestType = ServiceRequestType.MedicalBenefits;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.PostMajorityServices))
			serviceRequestType = ServiceRequestType.PostMajorityServices;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestServiceCfs))
			serviceRequestType = ServiceRequestType.RequestServiceCfs;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestServiceCapp))
			serviceRequestType = ServiceRequestType.RequestServiceCapp;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestForFamilySupport))
			serviceRequestType = ServiceRequestType.RequestForFamilySupport;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.RequestForInformation))
			serviceRequestType = ServiceRequestType.RequestForInformation;
		else if (EntityTypeExtensions.Matches(str, ServiceRequestTypeStrings.YouthServices))
			serviceRequestType = ServiceRequestType.YouthServices;
		else
			serviceRequestType = ServiceRequestType.Unknown;

		return serviceRequestType > ServiceRequestType.Unknown
			&& serviceRequestType <= ServiceRequestType.YouthServices;
	}
}
