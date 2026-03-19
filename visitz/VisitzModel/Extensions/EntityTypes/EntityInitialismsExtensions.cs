using VisitzModel.Models.EntityTypes;
using VisitzModel.Resources.Localization;

namespace VisitzModel.Extensions.EntityTypes;

public static class EntityInitialismsExtensions
{
    public static string GetDisplayInitials(this EntitySubtype entitySubtype)
    {
        return entitySubtype switch
        {
            EntitySubtype.AdoptionHome => EntityInitialisms.AdoptionHome,
            EntitySubtype.AdoptionService => EntityInitialisms.AdoptionService,
            EntitySubtype.AssessmentOnly => EntityInitialisms.AssessmentOnly,
            EntitySubtype.AssistedAdoption => EntityInitialisms.AssistedAdoption,
            EntitySubtype.Autism => EntityInitialisms.Autism,

            EntitySubtype.CysnFamilyServices => EntityInitialisms.CysnFamilyServices,
            EntitySubtype.ChildAtHomeProgram => EntityInitialisms.ChildAtHomeProgram,
            EntitySubtype.ChildServices => EntityInitialisms.ChildServices,
            EntitySubtype.FamilyServices => EntityInitialisms.FamilyServices,
            EntitySubtype.PostAdoptionService => EntityInitialisms.PostAdoptionService,
            EntitySubtype.Resource => EntityInitialisms.Resource,
            EntitySubtype.SupportNeedsChildrenYouth => EntityInitialisms.SupportNeedsChildrenYouth,
            EntitySubtype.ChildProtection => EntityInitialisms.ChildProtection,
            EntitySubtype.ReferralAndInquiry => EntityInitialisms.ReferralAndInquiry,

            EntitySubtype.AfterHoursAction => EntityInitialisms.AfterHoursAction,
            EntitySubtype.AfterHoursFrom => EntityInitialisms.AfterHoursFrom,
            EntitySubtype.AfterHoursInfo => EntityInitialisms.AfterHoursInfo,
            EntitySubtype.AgreementWithYoungAdults => EntityInitialisms.AgreementWithYoungAdults,
            EntitySubtype.Cysn => EntityInitialisms.Cysn,
            EntitySubtype.CentralizedServicesHub => EntityInitialisms.CentralizedServicesHub,
            EntitySubtype.Screening => EntityInitialisms.Screening,
            EntitySubtype.SupportNeedsRequest => EntityInitialisms.SupportNeedsRequest,

            EntitySubtype.AfterHours => EntityInitialisms.AfterHours,
            EntitySubtype.AutismFundingUnit => EntityInitialisms.AutismFundingUnit,
            EntitySubtype.CommunityDevelopment => EntityInitialisms.CommunityDevelopment,
            EntitySubtype.FamilyNeedsAssessment => EntityInitialisms.FamilyNeedsAssessment,
            EntitySubtype.RequestServiceCfs => EntityInitialisms.RequestServiceCfs,
            EntitySubtype.RequestServiceCapp => EntityInitialisms.RequestServiceCapp,
            EntitySubtype.RequestForFamilySupport => EntityInitialisms.RequestForFamilySupport,
            EntitySubtype.RequestForInformation => EntityInitialisms.RequestForInformation,
            EntitySubtype.YouthServices => EntityInitialisms.YouthServices,

            EntitySubtype.ProtocolInvestigation => EntityInitialisms.ProtocolInvestigation,
            EntitySubtype.MedicalBenefits => EntityInitialisms.MedicalBenefits,
            EntitySubtype.PostMajorityServices => EntityInitialisms.PostMajorityServices,
            EntitySubtype.Unknown => EntityInitialisms.Unknown,
            _ => throw new InvalidOperationException($"'{entitySubtype}' not supported"),
        };
    }
}
