using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    [ObservableProperty]
    public IEnumerable<FamilyMember> childrenInOutCare;

    // Using object instead of FamilyMember for generic as a workaround
    // see https://github.com/dotnet/maui/issues/8435#issuecomment-1365586648
    [ObservableProperty]
    public ObservableCollection<object> selectedChildren = [];

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

        SelectedChildren.CollectionChanged += SelectedChildren_CollectionChanged;

        var id = SubmitSafetyAssessmentService.MakeId(CaseloadItem);
        WeakReferenceMessenger.Default.Register(this, id);

        SafetyAssessment ??= await MakeNewSafetyAssessment();

        SetupFamilyNamePicker();
        SetupChildrenInOutCare();
    }

    private void SetupFamilyNamePicker()
    {
        var names = new SortedSet<string>();
        foreach (var member in CaseloadItem.FamilyMembers)
            names.Add(member.LastName);

        FamilyNames = names.AsList();

        TrySetSingularFamilyName();
    }

    private void SetupChildrenInOutCare()
    {
        ChildrenInOutCare = CaseloadItem.FamilyMembers;
    }

    public override void PageDestroyed()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        SelectedChildren.CollectionChanged -= SelectedChildren_CollectionChanged;

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
        SelectedChildren?.Clear();
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

    private void SelectedChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (FamilyMember child in e.NewItems.Cast<FamilyMember>())
                SafetyAssessment.ChildsInOutCare.Add(child.ContactId);
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (FamilyMember child in e.NewItems.Cast<FamilyMember>())
                SafetyAssessment.ChildsInOutCare.Remove(child.ContactId);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
            SafetyAssessment.ChildsInOutCare.Clear();
    }
}
