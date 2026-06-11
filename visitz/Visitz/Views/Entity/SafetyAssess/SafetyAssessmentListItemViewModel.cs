using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentListItemViewModel(SafetyAssessment safetyAssessment) : VisitzViewModel
{
    [ObservableProperty]
    public partial SafetyAssessment SafetyAssessment { get; set; } = safetyAssessment;
}
