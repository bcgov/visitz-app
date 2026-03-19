using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class EntitySubtypeExtensions
{
    public static string GetDisplayString(this EntitySubtype subtype)
    {
        return subtype switch
        {
            EntitySubtype.Unknown => EntityTypeStrings.Unknown,

            EntitySubtype.AdoptionHome => EntitySubtypeStrings.AdoptionHome,
            EntitySubtype.AdoptionService => EntitySubtypeStrings.AdoptionService,
            EntitySubtype.AssessmentOnly => EntitySubtypeStrings.AssessmentOnly,
            EntitySubtype.AssistedAdoption => EntitySubtypeStrings.AssistedAdoption,
            EntitySubtype.Autism => EntitySubtypeStrings.Autism,
            EntitySubtype.CysnFamilyServices => EntitySubtypeStrings.CysnFamilyServices,
            EntitySubtype.ChildAtHomeProgram => EntitySubtypeStrings.ChildAtHomeProgram,
            EntitySubtype.ChildServices => EntitySubtypeStrings.ChildServices,
            EntitySubtype.FamilyServices => EntitySubtypeStrings.FamilyServices,
            EntitySubtype.PostAdoptionService => EntitySubtypeStrings.PostAdoptionService,
            EntitySubtype.Resource => EntitySubtypeStrings.Resource,
            EntitySubtype.SupportNeedsChildrenYouth => EntitySubtypeStrings.SupportNeedsChildrenYouth,

            EntitySubtype.ChildProtection => EntitySubtypeStrings.ChildProtection,
            EntitySubtype.ReferralAndInquiry => EntitySubtypeStrings.ReferralAndInquiry,

            EntitySubtype.AfterHoursAction => EntitySubtypeStrings.AfterHoursAction,
            EntitySubtype.AfterHoursFrom => EntitySubtypeStrings.AfterHoursFrom,
            EntitySubtype.AfterHoursInfo => EntitySubtypeStrings.AfterHoursInfo,
            EntitySubtype.AgreementWithYoungAdults => EntitySubtypeStrings.AgreementWithYoungAdults,
            EntitySubtype.Cysn => EntitySubtypeStrings.Cysn,
            EntitySubtype.CentralizedServicesHub => EntitySubtypeStrings.CentralizedServicesHub,
            EntitySubtype.Screening => EntitySubtypeStrings.Screening,
            EntitySubtype.SupportNeedsRequest => EntitySubtypeStrings.SupportNeedsRequest,

            EntitySubtype.AfterHours => EntitySubtypeStrings.AfterHours,
            EntitySubtype.AutismFundingUnit => EntitySubtypeStrings.AutismFundingUnit,
            EntitySubtype.CommunityDevelopment => EntitySubtypeStrings.CommunityDevelopment,
            EntitySubtype.FamilyNeedsAssessment => EntitySubtypeStrings.FamilyNeedsAssessment,
            EntitySubtype.RequestServiceCfs => EntitySubtypeStrings.RequestServiceCfs,
            EntitySubtype.RequestServiceCapp => EntitySubtypeStrings.RequestServiceCapp,
            EntitySubtype.RequestForFamilySupport => EntitySubtypeStrings.RequestForFamilySupport,
            EntitySubtype.RequestForInformation => EntitySubtypeStrings.RequestForInformation,
            EntitySubtype.YouthServices => EntitySubtypeStrings.YouthServices,

            EntitySubtype.ProtocolInvestigation => EntitySubtypeStrings.ProtocolInvestigation,
            EntitySubtype.MedicalBenefits => EntitySubtypeStrings.MedicalBenefits,
            EntitySubtype.PostMajorityServices => EntitySubtypeStrings.PostMajorityServices,

            _ => throw new NotImplementedException(),
        };
    }

    public static EntitySubtype ParseEntitySubtype(this string str)
    {
        str = str.Trim();

        if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AdoptionHome))
            return EntitySubtype.AdoptionHome;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AdoptionService))
            return EntitySubtype.AdoptionService;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AssessmentOnly))
            return EntitySubtype.AssessmentOnly;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AssistedAdoption))
            return EntitySubtype.AssistedAdoption;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.Autism))
            return EntitySubtype.Autism;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.CysnFamilyServices))
            return EntitySubtype.CysnFamilyServices;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.ChildAtHomeProgram))
            return EntitySubtype.ChildAtHomeProgram;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.ChildServices))
            return EntitySubtype.ChildServices;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.FamilyServices))
            return EntitySubtype.FamilyServices;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.MedicalBenefits))
            return EntitySubtype.MedicalBenefits;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.PostAdoptionService))
            return EntitySubtype.PostAdoptionService;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.PostMajorityServices))
            return EntitySubtype.PostMajorityServices;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.Resource))
            return EntitySubtype.Resource;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.SupportNeedsChildrenYouth))
            return EntitySubtype.SupportNeedsChildrenYouth;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.ChildProtection))
            return EntitySubtype.ChildProtection;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.ProtocolInvestigation))
            return EntitySubtype.ProtocolInvestigation;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.ReferralAndInquiry))
            return EntitySubtype.ReferralAndInquiry;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AfterHoursAction))
            return EntitySubtype.AfterHoursAction;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AfterHoursFrom))
            return EntitySubtype.AfterHoursFrom;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AfterHoursInfo))
            return EntitySubtype.AfterHoursInfo;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AgreementWithYoungAdults))
            return EntitySubtype.AgreementWithYoungAdults;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.Cysn))
            return EntitySubtype.Cysn;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.CentralizedServicesHub))
            return EntitySubtype.CentralizedServicesHub;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.ProtocolInvestigation))
            return EntitySubtype.ProtocolInvestigation;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.Screening))
            return EntitySubtype.Screening;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.SupportNeedsRequest))
            return EntitySubtype.SupportNeedsRequest;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AfterHours))
            return EntitySubtype.AfterHours;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.AutismFundingUnit))
            return EntitySubtype.AutismFundingUnit;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.CommunityDevelopment))
            return EntitySubtype.CommunityDevelopment;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.FamilyNeedsAssessment))
            return EntitySubtype.FamilyNeedsAssessment;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.MedicalBenefits))
            return EntitySubtype.MedicalBenefits;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.PostMajorityServices))
            return EntitySubtype.PostMajorityServices;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.RequestServiceCfs))
            return EntitySubtype.RequestServiceCfs;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.RequestServiceCapp))
            return EntitySubtype.RequestServiceCapp;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.RequestForFamilySupport))
            return EntitySubtype.RequestForFamilySupport;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.RequestForInformation))
            return EntitySubtype.RequestForInformation;
        else if (EntityTypeExtensions.Matches(str, EntitySubtypeStrings.YouthServices))
            return EntitySubtype.YouthServices;
        else
            return EntitySubtype.Unknown;
    }

    public static bool TryParseEntitySubtype(this string str, out EntitySubtype subtype)
    {
        subtype = ParseEntitySubtype(str);
        return (subtype >= EntitySubtype.AdoptionHome && subtype <= EntitySubtype.SupportNeedsChildrenYouth)
            || (subtype >= EntitySubtype.ChildProtection && subtype <= EntitySubtype.ReferralAndInquiry)
            || (subtype >= EntitySubtype.AfterHoursAction && subtype <= EntitySubtype.SupportNeedsRequest)
            || (subtype >= EntitySubtype.AfterHours && subtype <= EntitySubtype.YouthServices)
            || (subtype >= EntitySubtype.ProtocolInvestigation && subtype <= EntitySubtype.PostMajorityServices);
    }
}
