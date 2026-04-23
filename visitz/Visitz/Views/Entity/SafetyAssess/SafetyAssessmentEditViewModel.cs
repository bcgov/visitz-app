using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oidc;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using VisitzModel;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentEditViewModel : VisitzViewModel, IBusinessObjectHolder
{
    public static readonly string SafetyDecisionGroup = "SafetyDecisionGroup";
    public static readonly string WhichChildrenPlaced = "WhichChildrenPlaced";

    [ObservableProperty]
    public DateTime maxDate = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public IBusinessObject businessObject;

    [ObservableProperty]
    public SafetyAssessment assessment;

    public SafetyAssessment ViewAssessment { get; set; }

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
    public IEnumerable<IcmContact> availableChildrenInOutCare;

    // Using object instead of IcmContact for generic as a workaround
    // see https://github.com/dotnet/maui/issues/8435#issuecomment-1365586648
    [ObservableProperty]
    public ObservableCollection<object> selectedChildren = [];

    [Obsolete("Workaround for RadioButton rendering issue https://github.com/dotnet/maui/issues/19437")]
    [ObservableProperty]
    public bool safeChecked;

    [ObservableProperty]
    public bool safeWithInterventionsChecked;

    [ObservableProperty]
    public bool unsafeChecked;

    [ObservableProperty]
    public bool allChildrenPlaced;

    [ObservableProperty]
    public bool someChildrenPlaced;

    [ObservableProperty]
    public bool canPublish;

    [ObservableProperty]
    public bool canDiscard;

    [ObservableProperty]
    public bool isReadOnly;

    private Realm Realm;

    public DraftSaveStateHandler SaveStateHandler { get; } = new();

    [ObservableProperty]
    private AssessmentDraft draftItem;

    // FIXME This is used to workaround DatePickers not being able to use null values.
    // https://github.com/dotnet/maui/issues/1100, https://github.com/dotnet/maui/pull/27921
    // We can revisit this after upgrading to MAUI 10.
    [ObservableProperty]
    public bool showDateOfAssessment;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        Realm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        SetupFamilyNamePicker();
        SetupChildrenInOutCare();

        if (IsReadOnly)
            Assessment = ViewAssessment;
        else
            await SetupAssessmentDraft();

        SetDatePickerVisibility();
        SelectedChildren.CollectionChanged += SelectedChildren_CollectionChanged;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            SaveStateHandler.Dispose();
            WeakReferenceMessenger.Default.UnregisterAll(this);

            SelectedChildren.CollectionChanged -= SelectedChildren_CollectionChanged;
            UnsubscribeFromAssessment();

            Assessment = null;

            Realm?.Dispose();
            Realm = null;

            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void SetDatePickerVisibility()
    {
        bool IsEditable = !IsReadOnly;
        ShowDateOfAssessment = IsEditable || Assessment.DateOfAssessmentBinding != null;
    }

    private async Task<SafetyAssessment> MakeNewSafetyAssessment()
    {
        var info = await OidcSessionInfo.GetAsync();

        return SafetyAssessment.Make(BusinessObject.FileNumber, info.Idir, BusinessObject.GetKeyPlayer().LastName);
    }

    private async Task SetupAssessmentDraft()
    {
        DraftItem = null;
        Assessment =
            SafetyAssessment.FindByIncidentNumber(Realm, BusinessObject.FileNumber) ?? await MakeNewSafetyAssessment();

        await TryAssociateDraftItem();

        UpdateCanPublish();
        SubscribeToAssessment();
    }

    private async Task TryAssociateDraftItem()
    {
        if (Assessment.IsManaged)
            DraftItem = await AssessmentDraft.Upsert(Realm, Assessment, BusinessObject.DisplayName);
    }

    private async void Assessment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = TrySendSavedMessage(DraftSaveState.Saving);

        if (!Assessment.IsManaged)
            DraftItem = await AssessmentDraft.Upsert(Realm, Assessment, BusinessObject.DisplayName);
        else if (DraftItem?.IsValid ?? false)
            DraftItem.LastUpdatedBinding = DateTimeOffset.Now;

        CanDiscard = Assessment.IsManaged;
        UpdateCanPublish();
    }

    private void UpdateCanPublish()
    {
        CanPublish = Factors.AllAnswered && Decisions.IsAnswered && IsSelectedChildrenValid();
    }

    private bool IsSelectedChildrenValid()
    {
        return Decisions.Decision != SafetyDecisionOption.Unsafe
            || Decisions.DecisionUnsafe != SafetyDecisions.SomeChildrenPlaced
            || Assessment.ChildsInOutCare.Any();
    }

    private void SetupFamilyNamePicker()
    {
        var names = new SortedSet<string>();
        foreach (var member in BusinessObject.GetContacts())
            names.Add(member.LastName);

        FamilyNames = names.AsList();
    }

    private void SetupChildrenInOutCare()
    {
        AvailableChildrenInOutCare = BusinessObject.GetContacts();
    }

    partial void OnAssessmentChanged(SafetyAssessment value)
    {
        SetupBindings(value);
    }

    private void SetupBindings(SafetyAssessment value)
    {
        if (FamilyNames?.Count > 0 && value != null && !value.IsManaged)
            value.FamilyName = FamilyNames[0];

        Influence = value?.FactorInfluence;
        Capacity = value?.ProtectiveCapacity;
        Decisions = value?.SafetyDecisions;
        Factors = value?.SafetyFactors;
        Interventions = value?.SafetyInterventions;

        if (value == null)
            SelectedChildren.Clear();
        else if (AvailableChildrenInOutCare != null)
        {
            foreach (var child in AvailableChildrenInOutCare)
                if (value.ChildsInOutCare.Contains(child.Id))
                    SelectedChildren.Add(child);
        }

        CanDiscard = value?.IsManaged ?? false;
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
        {
            ClearDecisionBools();
            ClearDecisionUnsafeBools();
            Decisions.PropertyChanged -= Assessment_PropertyChanged;
        }

        if (Factors != null)
            Factors.PropertyChanged -= Assessment_PropertyChanged;

        if (Interventions != null)
            Interventions.PropertyChanged -= Assessment_PropertyChanged;
    }

    [RelayCommand]
    public async Task Publish()
    {
#if DEBUG
        WriteSafetyAssessmentJson();
#endif
        var saPublishVm = ServiceProvider.Current.GetService<SafetyAssessmentPublishViewModel>();
        saPublishVm.BusinessObject = BusinessObject;

        var saPublish = new PublishPage(saPublishVm);
        await Navigator.Navigation.PushAsync(saPublish);
        await Navigator.Navigation.PopModalAsync();
    }

    [RelayCommand]
    public async Task Reset()
    {
        UnsubscribeFromAssessment();
        await AssessmentDraft.TryDeleteAsync(Assessment);

        await TrySendSavedMessage(DraftSaveState.None);

        await SetupAssessmentDraft();
        SelectedChildren?.Clear();
    }

#if DEBUG
    private void WriteSafetyAssessmentJson()
    {
        var entity = Assessment.ToApiJson();

#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        var json = System.Text.Json.JsonSerializer.Serialize(
            entity,
            new System.Text.Json.JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            }
        );
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances

        ConsoleTrace.TraceMethod(this, json);
    }
#endif

    private void SelectedChildren_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Assessment.Commit(() =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IcmContact child in e.NewItems.Cast<IcmContact>())
                    Assessment.ChildsInOutCare.Add(child.Id);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IcmContact child in e.OldItems.Cast<IcmContact>())
                    Assessment.ChildsInOutCare.Remove(child.Id);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
                Assessment.ChildsInOutCare.Clear();

            _ = TrySendSavedMessage(DraftSaveState.Saving);
        });

        UpdateCanPublish();
    }

    private async Task TrySendSavedMessage(DraftSaveState state)
    {
        if (state == DraftSaveState.None)
            SaveStateHandler.Clear();
        else if (state == DraftSaveState.Saving && Assessment.IsManaged)
            await SaveStateHandler.Saving();
    }

    private void ClearDecisionBools()
    {
        SafeChecked = false;
        SafeWithInterventionsChecked = false;
        UnsafeChecked = false;
    }

    private void ClearDecisionUnsafeBools()
    {
        AllChildrenPlaced = false;
        SomeChildrenPlaced = false;
    }

    partial void OnUnsafeCheckedChanged(bool value)
    {
        if (!value)
            ClearDecisionUnsafeBools();
    }

    partial void OnAllChildrenPlacedChanged(bool value)
    {
        SelectedChildren.Clear();

        if (value)
            foreach (var child in AvailableChildrenInOutCare)
                SelectedChildren.Add(child);
    }
}
