using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Extensions;
using Visitz.Models;
using Visitz.Models.SafetyAssess;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessViewModel : VisitzViewModel, ICaseloadItemHolder, IRecipient<ServiceStateMessage>
{
    public static readonly string SafetyDecisionGroup = "SafetyDecisionGroup";
    public static readonly string WhichChildrenPlaced = "WhichChildrenPlaced";

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public SafetyAssessment safetyAssessment;

    [ObservableProperty]
    public IList<string> familyNames;

    private async Task<SafetyAssessment> MakeNewSafetyAssessment()
    {
        var info = await VisitzSessionInfo.GetAsync();
        return new SafetyAssessment()
        {
            IncidentNumber = CaseloadItem.CaseIncidentNumber,
            WorkerId = info.FirstLastName,
            FamilyName = CaseloadItem.KeyPlayerLastName,
            Operation = LocalizedStrings.Insert,
            FactorInfluence = new FactorInfluence(),
            SafetyFactors = new SafetyFactors(),
            ProtectiveCapacity = new ProtectiveCapacity(),
            SafetyInterventions = new SafetyInterventions(),
            SafetyDecisions = new SafetyDecisions(),
        };
    }

    public override async void PageCreated()
    {
        base.PageCreated();

        var id = SubmitSafetyAssessmentService.MakeId(CaseloadItem);
        WeakReferenceMessenger.Default.Register(this, id);

        SafetyAssessment ??= await MakeNewSafetyAssessment();

        var names = new SortedSet<string>();
        foreach (var member in CaseloadItem.FamilyMembers)
            names.Add(member.LastName);

        FamilyNames = names.AsList();

        TrySetSingularFamilyName();
    }

    public override void PageDestroyed()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        base.PageDestroyed();
    }

    [RelayCommand]
    public void Publish()
    {
#if DEBUG
        WriteSafetyAssessmentJson();
#endif
        var msg = SubmitSafetyAssessmentService.MakeStartMessage(SafetyAssessment);
        WeakReferenceMessenger.Default.Send(msg);
    }

    [RelayCommand]
    public async void Reset()
    {
        SafetyAssessment = await MakeNewSafetyAssessment();
    }

#if DEBUG
    private void WriteSafetyAssessmentJson()
    {
        var entity = SafetyAssessment.ToApiEntity();

        var json = System.Text.Json.JsonSerializer.Serialize(entity, new System.Text.Json.JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });

        ConsoleTrace.TraceMethod(this, json);
    }
#endif

    private void TrySetSingularFamilyName()
    {
        if (FamilyNames.Count == 1)
            SafetyAssessment.FamilyName = FamilyNames[0];
    }

    public void Receive(ServiceStateMessage message)
    {
        // TODO: Tasks upon API completion
    }
}
