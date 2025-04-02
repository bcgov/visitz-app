using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;

namespace VisitzModel.Models.SafetyAssess;

public partial class SafetyFactors : IRealmObject
{
    public bool? PhysicalHarm { get; set; }
        
    public bool SeriousInjuryAbuse { get; set; }
        
    public bool FearsMaltreatChild { get; set; }
        
    public bool ThreatAgainstChild { get; set; }
        
    public bool ExcessiveForce { get; set; }
        
    public bool SubsExposedInfant { get; set; }

    public string CmtClarification { get; set; } = string.Empty;

    public bool? CurrentCircumstances { get; set; }

    public string CmtCircumstances { get; set; } = string.Empty;

    public bool? SexAbuse { get; set; }

    public string CmtAbuse { get; set; } = string.Empty;

    public bool? UnableToProtect { get; set; }

    public string CmtProtect { get; set; } = string.Empty;

    public bool? InjuryExplanation { get; set; }

    public string CmtExplanation { get; set; } = string.Empty;

    public bool? RefuseAccess { get; set; }

    public string CmtAccess { get; set; } = string.Empty;

    public bool? ImmediateNeeds { get; set; }

    public string CmtNeeds { get; set; } = string.Empty;

    public bool? PhysicalCondition { get; set; }

    public string CmtCondition { get; set; } = string.Empty;

    public bool? CurrentAbuse { get; set; }

    public string CmtCurrent { get; set; } = string.Empty;

    public bool? PartnerViolence { get; set; }

    public string CmtViolence { get; set; } = string.Empty;

    public bool? PredominantlyNegative { get; set; }

    public string CmtNegative { get; set; } = string.Empty;

    public bool? EmotionalStability { get; set; }

    public string CmtEmotional { get; set; } = string.Empty;

    public bool? ChildFearful { get; set; }

    public string CmtFearful { get; set; } = string.Empty;

    public bool? OtherFactors { get; set; }

    public string CmtOtherFactors { get; set; } = string.Empty;

    /// <summary>
    /// Unused
    /// </summary>
    public bool? CurretAbuse { get; set; }

    public bool AnyTrue => (PhysicalHarm ?? false)
        || (CurrentCircumstances ?? false)
        || (SexAbuse ?? false)
        || (UnableToProtect ?? false)
        || (InjuryExplanation ?? false)
        || (RefuseAccess ?? false)
        || (ImmediateNeeds ?? false)
        || (PhysicalCondition ?? false)
        || (CurrentAbuse ?? false)
        || (PartnerViolence ?? false)
        || (PredominantlyNegative ?? false)
        || (EmotionalStability ?? false)
        || (ChildFearful ?? false)
        || (OtherFactors ?? false);

    public bool AllAnswered => PhysicalHarm != null
        && CurrentCircumstances != null
        && SexAbuse != null
        && UnableToProtect != null
        && InjuryExplanation != null
        && RefuseAccess != null
        && ImmediateNeeds != null
        && PhysicalCondition != null
        && CurrentAbuse != null
        && PartnerViolence != null
        && PredominantlyNegative != null
        && EmotionalStability != null
        && ChildFearful != null
        && OtherFactors != null;

    public static SafetyFactors FromApiEntity(SubmitSafetyFactorsJson entity)
    {
        return new SafetyFactors()
        {
            PhysicalHarm = entity.PhysicalHarm.ParseWordTruthiness(),
            SeriousInjuryAbuse = entity.SeriousInjuryAbuse.ParseWordTruthiness(),
            FearsMaltreatChild = entity.FearsMaltreatChild.ParseWordTruthiness(),
            ThreatAgainstChild = entity.ThreatAgainstChild.ParseWordTruthiness(),
            ExcessiveForce = entity.ExcessiveForce.ParseWordTruthiness(),
            SubsExposedInfant = entity.SubsExposedInfant.ParseWordTruthiness(),
            CmtClarification = entity.CmtClarification,
            CurrentCircumstances = entity.CurrentCircumstances.ParseWordTruthiness(),
            CmtCircumstances = entity.CmtCircumstances,
            SexAbuse = entity.SexAbuse.ParseWordTruthiness(),
            CmtAbuse = entity.CmtAbuse,
            UnableToProtect = entity.UnableToProtect.ParseWordTruthiness(),
            CmtProtect = entity.CmtProtect,
            InjuryExplanation = entity.InjuryExplanation.ParseWordTruthiness(),
            CmtExplanation = entity.CmtExplanation,
            RefuseAccess = entity.RefuseAccess.ParseWordTruthiness(),
            CmtAccess = entity.CmtAccess,
            ImmediateNeeds = entity.ImmediateNeeds.ParseWordTruthiness(),
            CmtNeeds = entity.CmtNeeds,
            PhysicalCondition = entity.PhysicalCondition.ParseWordTruthiness(),
            CmtCondition = entity.CmtCondition,
            CurrentAbuse = entity.CurrentAbuse.ParseWordTruthiness(),
            CmtCurrent = entity.CmtCurrent,
            PartnerViolence = entity.PartnerViolence.ParseWordTruthiness(),
            CmtViolence = entity.CmtViolence,
            PredominantlyNegative = entity.PredominantlyNegative.ParseWordTruthiness(),
            CmtNegative = entity.CmtNegative,
            EmotionalStability = entity.EmotionalStability.ParseWordTruthiness(),
            CmtEmotional = entity.CmtEmotional,
            ChildFearful = entity.ChildFearful.ParseWordTruthiness(),
            CmtFearful = entity.CmtFearful,
            OtherFactors = entity.OtherFactors.ParseWordTruthiness(),
            CmtOtherFactors = entity.CmtOtherFactors,
            CurretAbuse = entity.CurretAbuse.ParseWordTruthiness(),
        };
    }

    public SubmitSafetyFactorsJson ToApiEntity()
    {
        return new SubmitSafetyFactorsJson()
        {
            PhysicalHarm = PhysicalHarm?.AsTruthyWord(),
            SeriousInjuryAbuse = SeriousInjuryAbuse.AsTruthyChar(),
            FearsMaltreatChild = FearsMaltreatChild.AsTruthyChar(),
            ThreatAgainstChild = ThreatAgainstChild.AsTruthyChar(),
            ExcessiveForce = ExcessiveForce.AsTruthyChar(),
            SubsExposedInfant = SubsExposedInfant.AsTruthyChar(),
            CmtClarification = CmtClarification,
            CurrentCircumstances = CurrentCircumstances?.AsTruthyWord(),
            CmtCircumstances = CmtCircumstances,
            SexAbuse = SexAbuse?.AsTruthyWord(),
            CmtAbuse = CmtAbuse,
            UnableToProtect = UnableToProtect?.AsTruthyWord(),
            CmtProtect = CmtProtect,
            InjuryExplanation = InjuryExplanation?.AsTruthyWord(),
            CmtExplanation = CmtExplanation,
            RefuseAccess = RefuseAccess?.AsTruthyWord(),
            CmtAccess = CmtAccess,
            ImmediateNeeds = ImmediateNeeds?.AsTruthyWord(),
            CmtNeeds = CmtNeeds,
            PhysicalCondition = PhysicalCondition?.AsTruthyWord(),
            CmtCondition = CmtCondition,
            CurrentAbuse = CurrentAbuse?.AsTruthyWord(),
            CmtCurrent = CmtCurrent,
            PartnerViolence = PartnerViolence?.AsTruthyWord(),
            CmtViolence = CmtViolence,
            PredominantlyNegative = PredominantlyNegative?.AsTruthyWord(),
            CmtNegative = CmtNegative,
            EmotionalStability = EmotionalStability?.AsTruthyWord(),
            CmtEmotional = CmtEmotional,
            ChildFearful = ChildFearful?.AsTruthyWord(),
            CmtFearful = CmtFearful,
            OtherFactors = OtherFactors?.AsTruthyWord(),
            CmtOtherFactors = CmtOtherFactors,
            CurretAbuse = CurretAbuse?.AsTruthyWord(),
        };
    }
}
