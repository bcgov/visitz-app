using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Models.SafetyAssess;
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

    [ObservableProperty]
    public string workerId;

    public override async void PageCreated()
    {
        base.PageCreated();

        SafetyAssessment ??= new SafetyAssessment()
        {
            FactorInfluence = new FactorInfluence(),
            SafetyFactors = new SafetyFactors(),
            ProtectiveCapacity = new ProtectiveCapacity(),
            SafetyInterventions = new SafetyInterventions(),
            SafetyDecisions = new SafetyDecisions(),
        };

        var info = await VisitzSessionInfo.GetAsync();
        WorkerId = info.Idir;
    }

    [RelayCommand]
    public void Publish()
    {
        var msg = SubmitSafetyAssessmentService.MakeStartMessage(SafetyAssessment);
        WeakReferenceMessenger.Default.Send(msg);
    }
}
