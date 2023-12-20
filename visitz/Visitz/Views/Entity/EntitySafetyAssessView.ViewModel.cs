using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Models.SafetyAssess;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessViewModel : VisitzViewModel, ICaseloadItemHolder
{
    public static readonly string SafetyDecisionGroup = "SafetyDecisionGroup";
    public static readonly string WhichChildrenPlaced = "WhichChildrenPlaced";

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public SafetyAssessment safetyAssessment;

    public override async void PageCreated()
    {
        base.PageCreated();

        var info = await VisitzSessionInfo.GetAsync();
        SafetyAssessment ??= new SafetyAssessment()
        {
            IncidentNumber = CaseloadItem.CaseIncidentNumber,
            WorkerId = info.Idir,
            FamilyName = CaseloadItem.KeyPlayerLastName,
            Operation = LocalizedStrings.Insert,
            FactorInfluence = new FactorInfluence(),
            SafetyFactors = new SafetyFactors(),
            ProtectiveCapacity = new ProtectiveCapacity(),
            SafetyInterventions = new SafetyInterventions(),
            SafetyDecisions = new SafetyDecisions(),
        };
    }

    [RelayCommand]
    public void Publish()
    {
        var msg = SubmitSafetyAssessmentService.MakeStartMessage(SafetyAssessment);
        WeakReferenceMessenger.Default.Send(msg);
    }
}
