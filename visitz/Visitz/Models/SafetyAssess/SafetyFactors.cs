using Realms;
using Visitz.Extensions;
using VisitzApi.Models.SafetyAssess;

namespace Visitz.Models.SafetyAssess;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", 
    Justification = "Property naming style recommended by Realm.NET.")]
public partial class SafetyFactors : IRealmObject
{
    private const int CommentsMaxLength = 1000;

    public bool PhysicalHarm { get; set; }
        
    public bool SeriousInjuryAbuse { get; set; }
        
    public bool FearsMaltreatChild { get; set; }
        
    public bool ThreatAgainstChild { get; set; }
        
    public bool ExcessiveForce { get; set; }
        
    public bool SubsExposedInfant { get; set; }

    [MapTo(nameof(CmtClarification))]
    private string cmtClarification {  get; set; }

    public string CmtClarification 
    {
        get => cmtClarification;
        set => cmtClarification = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool CurrentCircumstances { get; set; }

    [MapTo(nameof(CmtCircumstances))]
    private string cmtCircumstances{ get; set; }
    public string CmtCircumstances
    {
        get => cmtCircumstances;
        set => cmtCircumstances = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool SexAbuse { get; set; }

    [MapTo(nameof(CmtAbuse))]
    private string cmtAbuse{ get; set; }
    public string CmtAbuse
    {
        get => cmtAbuse;
        set => cmtAbuse = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool UnableToProtect { get; set; }

    [MapTo(nameof(CmtProtect))]
    private string cmtProtect{ get; set; }
    public string CmtProtect
    {
        get => cmtProtect;
        set => cmtProtect = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool InjuryExplanation { get; set; }

    [MapTo(nameof(CmtExplanation))]
    private string cmtExplanation{ get; set; }
    public string CmtExplanation
    {
        get => cmtExplanation;
        set => cmtExplanation = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool RefuseAccess { get; set; }

    [MapTo(nameof(CmtAccess))]
    private string cmtAccess{ get; set; }
    public string CmtAccess
    {
        get => cmtAccess;
        set => cmtAccess = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool ImmediateNeeds { get; set; }

    [MapTo(nameof(CmtNeeds))]
    private string cmtNeeds{ get; set; }
    public string CmtNeeds
    {
        get => cmtNeeds;
        set => cmtNeeds = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool PhysicalCondition { get; set; }

    [MapTo(nameof(CmtCondition))]
    private string cmtCondition{ get; set; }
    public string CmtCondition
    {
        get => cmtCondition;
        set => cmtCondition = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool CurrentAbuse { get; set; }

    [MapTo(nameof(CmtCurrent))]
    private string cmtCurrent{ get; set; }
    public string CmtCurrent
    {
        get => cmtCurrent;
        set => cmtCurrent = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool PartnerViolence { get; set; }

    [MapTo(nameof(CmtViolence))]
    private string cmtViolence{ get; set; }
    public string CmtViolence
    {
        get => cmtViolence;
        set => cmtViolence = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool PredominantlyNegative { get; set; }

    [MapTo(nameof(CmtNegative))]
    private string cmtNegative{ get; set; }
    public string CmtNegative
    {
        get => cmtNegative;
        set => cmtNegative = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool EmotionalStability { get; set; }

    [MapTo(nameof(CmtEmotional))]
    private string cmtEmotional{ get; set; }
    public string CmtEmotional
    {
        get => cmtEmotional;
        set => cmtEmotional = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool ChildFearful { get; set; }

    [MapTo(nameof(CmtFearful))]
    private string cmtFearful{ get; set; }
    public string CmtFearful
    {
        get => cmtFearful;
        set => cmtFearful = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool OtherFactors { get; set; }

    [MapTo(nameof(CmtOtherFactors))]
    private string cmtOtherFactors{ get; set; }
    public string CmtOtherFactors
    {
        get => cmtOtherFactors;
        set => cmtOtherFactors = value?.TruncateEnd(CommentsMaxLength);
    }

    public bool CurretAbuse { get; set; }

    public static SafetyFactors FromApiEntity(SafetyFactorsEntity entity)
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
}
