using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Visitz.Authentication.Keycloak;
using Visitz.Extensions;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Models.SafetyAssess;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Storage;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntitySafetyAssessViewModel : VisitzViewModel, ICaseloadItemHolder, IRecipient<ServiceStateMessage>
{
    public static readonly string SafetyDecisionGroup = "SafetyDecisionGroup";
    public static readonly string WhichChildrenPlaced = "WhichChildrenPlaced";

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public SafetyAssessment assessment;

    [ObservableProperty]
    public FactorInfluence influence;

    [ObservableProperty]
    public ProtectiveCapacity capacity;

    [ObservableProperty]
    public SafetyDecisions decisions;

    [ObservableProperty]
    public SafetyFactors factors;

    [ObservableProperty]
    public SafetyInterventions interventions;

    [ObservableProperty]
    public IList<string> familyNames;

    [ObservableProperty]
    public IEnumerable<FamilyMember> childrenInOutCare;

    // Using object instead of FamilyMember for generic as a workaround
    // see https://github.com/dotnet/maui/issues/8435#issuecomment-1365586648
    [ObservableProperty]
    public ObservableCollection<object> selectedChildren = [];

    private Realm Realm;

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

        Realm = await VisitzRealm.GetSafetyAssessmentDraftAsync();
        SetupFamilyNamePicker();
        SetupChildrenInOutCare();
        await SetupAssessment();
    }

    private async Task SetupAssessment()
    {
        UnsubscribeFromAssessment();

        if (SafetyAssessment.FindByIncidentNumber(Realm, CaseloadItem.CaseIncidentNumber) is SafetyAssessment sa)
            Assessment = sa;
        else
            Assessment = await MakeNewSafetyAssessment();
    }

    private async void Assessment_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (!Assessment.IsManaged)
            await Assessment.Save(Realm);

        var msg = new DraftSavedMessage<DraftSavedView.State>(DraftSavedView.State.Saving);
        StrongReferenceMessenger.Default.Send(msg);
    }

    private void SetupFamilyNamePicker()
    {
        var names = new SortedSet<string>();
        foreach (var member in CaseloadItem.FamilyMembers)
            names.Add(member.LastName);

        FamilyNames = names.AsList();
    }

    private void SetupChildrenInOutCare()
    {
        ChildrenInOutCare = CaseloadItem.FamilyMembers;
    }

    public override void PageDestroyed()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

        UnsubscribeFromAssessment();
        SelectedChildren.CollectionChanged -= SelectedChildren_CollectionChanged;

        base.PageDestroyed();
    }

    partial void OnAssessmentChanged(SafetyAssessment value)
    {
        if (FamilyNames.Count > 0 && !value.IsManaged)
            value.FamilyName = FamilyNames[0];

        Influence = value.FactorInfluence;
        Capacity = value.ProtectiveCapacity;
        Decisions = value.SafetyDecisions;
        Factors = value.SafetyFactors;
        Interventions = value.SafetyInterventions;

        foreach (var child in ChildrenInOutCare)
            if (value.ChildsInOutCare.Contains(child.ContactId))
                SelectedChildren.Add(child);

        SubscribeToAssessment();
    }

    private void SubscribeToAssessment()
    {
        Assessment.PropertyChanged += Assessment_PropertyChanged;
        Influence.PropertyChanged += Assessment_PropertyChanged;
        Capacity.PropertyChanged += Assessment_PropertyChanged;
        Decisions.PropertyChanged += Assessment_PropertyChanged;
        Factors.PropertyChanged += Assessment_PropertyChanged;
        Interventions.PropertyChanged += Assessment_PropertyChanged;
    }

    private void UnsubscribeFromAssessment()
    {
        if (Assessment != null)
            Assessment.PropertyChanged -= Assessment_PropertyChanged;

        if (Influence != null)
            Influence.PropertyChanged -= Assessment_PropertyChanged;

        if (Capacity != null)
            Capacity.PropertyChanged -= Assessment_PropertyChanged;

        if (Decisions != null)
            Decisions.PropertyChanged -= Assessment_PropertyChanged;

        if (Factors != null)
            Factors.PropertyChanged -= Assessment_PropertyChanged;

        if (Interventions != null)
            Interventions.PropertyChanged -= Assessment_PropertyChanged;
    }

    [RelayCommand]
    public void Publish()
    {
#if DEBUG
        WriteSafetyAssessmentJson();
#endif
        var msg = SubmitSafetyAssessmentService.MakeStartMessage(Assessment);
        WeakReferenceMessenger.Default.Send(msg);
    }

    [RelayCommand]
    public async void Reset()
    {
        await Realm.WriteAsync(() => Realm.Remove(Assessment));

        var msg = new DraftSavedMessage<DraftSavedView.State>(DraftSavedView.State.None);
        StrongReferenceMessenger.Default.Send(msg);

        await SetupAssessment();
        SelectedChildren?.Clear();
    }

#if DEBUG
    private void WriteSafetyAssessmentJson()
    {
        var entity = Assessment.ToApiEntity();

        var json = System.Text.Json.JsonSerializer.Serialize(entity, new System.Text.Json.JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        });

        ConsoleTrace.TraceMethod(this, json);
    }
#endif

    public void Receive(ServiceStateMessage message)
    {
        // TODO: Tasks upon API completion
    }

    private void SelectedChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Assessment.Commit(() =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FamilyMember child in e.NewItems.Cast<FamilyMember>())
                    Assessment.ChildsInOutCare.Add(child.ContactId);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (FamilyMember child in e.OldItems.Cast<FamilyMember>())
                    Assessment.ChildsInOutCare.Remove(child.ContactId);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
                Assessment.ChildsInOutCare.Clear();
        });
    }
}
