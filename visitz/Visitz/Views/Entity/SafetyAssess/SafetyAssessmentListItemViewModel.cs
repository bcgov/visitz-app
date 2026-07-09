using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Localization;
using Visitz.Resources.Styles;
using Visitz.Views.BaseClasses;
using VisitzModel.Formats;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentListItemViewModel(SafetyAssessment safetyAssessment) : VisitzViewModel
{
    [ObservableProperty]
    public partial SafetyAssessment SafetyAssessment { get; set; } = safetyAssessment;

    public string CreatedDate =>
        string.Format(
            LocalizedStrings.DateLabel,
            SafetyAssessment.CreatedDateBinding.ToString(IcmDateFormats.BasicTimestampShort)
        );

    public string CreatedBy => string.Format(LocalizedStrings.CreatedByLabel, SafetyAssessment.WorkerId);

    public string TagText => SafetyAssessment.IsApproved ? LocalizedStrings.Approved : LocalizedStrings.InProgress;

    public Color TagBackgroundColor =>
        SafetyAssessment.IsApproved ? VisitzColors.TagGreenBackground : VisitzColors.UnpublishedDraftBackground;

    public Color TagTextColor =>
        SafetyAssessment.IsApproved ? VisitzColors.TagGreenText : VisitzColors.UnpublishedDraftTextColor;
}
