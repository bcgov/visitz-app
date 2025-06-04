namespace VisitzModel.Models.EntityTypes;

public enum EntitySubtype
{
    Unknown = 0,

    // Cases
    AdoptionHome = 1,
    AdoptionService = 2,
    AssessmentOnly = 3,
    AssistedAdoption = 4,
    Autism = 5,
    CysnFamilyServices = 6,
    ChildAtHomeProgram = 7,
    ChildServices = 8,
    FamilyServices = 9,
    PostAdoptionService = 10,
    Resource = 11,
    SupportNeedsChildrenYouth = 12,

    // Incidents
    ChildProtection = 13,
    ReferralAndInquiry = 14,

    // Memos
    AfterHoursAction = 15,
    AfterHoursFrom = 16,
    AfterHoursInfo = 17,
    AgreementWithYoungAdults = 18,
    Cysn = 19,
    CentralizedServicesHub = 20,
    Screening = 21,
    SupportNeedsRequest = 22,

    // Service Requests
    AfterHours = 23,
    AutismFundingUnit = 24,
    CommunityDevelopment = 25,
    FamilyNeedsAssessment = 26,
    RequestServiceCfs = 27,
    RequestServiceCapp = 28,
    RequestForFamilySupport = 29,
    RequestForInformation = 30,
    YouthServices = 31,

    // Common
    ProtocolInvestigation = 32,
    MedicalBenefits = 33,
    PostMajorityServices = 34,
}
