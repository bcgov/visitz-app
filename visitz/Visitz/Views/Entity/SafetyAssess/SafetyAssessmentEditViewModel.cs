using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

public partial class SafetyAssessmentEditViewModel : IcmRecordViewModel
{
    public static readonly string SafetyDecisionGroup = "SafetyDecisionGroup";
    public static readonly string WhichChildrenPlaced = "WhichChildrenPlaced";

    [ObservableProperty]
    public partial DateTime MaxDate { get; set; } = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public partial SafetyAssessment Assessment { get; set; } = new();
    public SafetyAssessment? ViewAssessment { get; set; }

    [ObservableProperty]
    public partial FactorInfluence Influence { get; set; } = new();

    [ObservableProperty]
    public partial ProtectiveCapacity Capacity { get; set; } = new();

    [ObservableProperty]
    public partial SafetyDecisions Decisions { get; set; } = new();

    [ObservableProperty]
    public partial SafetyFactors Factors { get; set; } = new();

    [ObservableProperty]
    public partial SafetyInterventions Interventions { get; set; } = new();

    [ObservableProperty]
    public partial IList<string> FamilyNames { get; set; } = [];

    [ObservableProperty]
    public partial IEnumerable<IcmContact> AvailableChildrenInOutCare { get; set; } = [];

    // Using object instead of IcmContact for generic as a workaround
    // see https://github.com/dotnet/maui/issues/8435#issuecomment-1365586648
    [ObservableProperty]
    public partial ObservableCollection<object> SelectedChildren { get; set; } = [];

    [field: Obsolete("Workaround for RadioButton rendering issue https://github.com/dotnet/maui/issues/19437")]
    [ObservableProperty]
    public partial bool SafeChecked { get; set; }

    [ObservableProperty]
    public partial bool SafeWithInterventionsChecked { get; set; }

    [ObservableProperty]
    public partial bool UnsafeChecked { get; set; }

    [ObservableProperty]
    public partial bool AllChildrenPlaced { get; set; }

    [ObservableProperty]
    public partial bool SomeChildrenPlaced { get; set; }

    [ObservableProperty]
    public partial bool CanPublish { get; set; }

    [ObservableProperty]
    public partial bool CanDiscard { get; set; }

    [ObservableProperty]
    public partial bool IsReadOnly { get; set; }

    [ObservableProperty]
    public partial DraftSaveState DraftSaveState { get; set; } = DraftSaveState.None;

    private Realm? DraftRealm;

    public DraftSaveStateHandler SaveStateHandler { get; } = new();

    [ObservableProperty]
    public partial AssessmentDraft? DraftItem { get; set; }

    // FIXME This is used to workaround DatePickers not being able to use null values.
    // https://github.com/dotnet/maui/issues/1100, https://github.com/dotnet/maui/pull/27921
    // We can revisit this after upgrading to MAUI 10.
    [ObservableProperty]
    public partial bool ShowDateOfAssessment { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        DraftRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        SetupFamilyNamePicker();
        SetupChildrenInOutCare();

        if (IsReadOnly && ViewAssessment != null)
            Assessment = ViewAssessment;
        else
            await SetupAssessmentDraft();

        SetDatePickerVisibility();
        SelectedChildren.CollectionChanged += SelectedChildren_CollectionChanged;

        SaveStateHandler.SaveStateChanged += SaveStateHandler_SaveStateChanged;
        SaveStateHandler.Clear();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            SaveStateHandler.SaveStateChanged -= SaveStateHandler_SaveStateChanged;
            SaveStateHandler.Dispose();
            WeakReferenceMessenger.Default.UnregisterAll(this);

            SelectedChildren.CollectionChanged -= SelectedChildren_CollectionChanged;
            UnsubscribeFromAssessment();

            Assessment = new()
            {
                FactorInfluence = new(),
                ProtectiveCapacity = new(),
                SafetyDecisions = new(),
                SafetyFactors = new(),
                SafetyInterventions = new(),
            };

            DraftRealm?.Dispose();
            DraftRealm = null;

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

        return SafetyAssessment.Make(
            BusinessObject.FileNumber,
            info.Idir,
            BusinessObject.GetKeyPlayer()?.LastName ?? string.Empty
        );
    }

    private async Task SetupAssessmentDraft()
    {
        if (DraftRealm == null)
            return;

        DraftItem = new();
        Assessment =
            SafetyAssessment.FindByIncidentNumber(DraftRealm, BusinessObject.FileNumber)
            ?? await MakeNewSafetyAssessment();

        await TryAssociateDraftItem();

        UpdateCanPublish();
        SubscribeToAssessment();
    }

    private async Task TryAssociateDraftItem()
    {
        if (Assessment.IsManaged && DraftRealm != null)
            DraftItem = await AssessmentDraft.Upsert(DraftRealm, Assessment, BusinessObject.DisplayName);
    }

    private async void Assessment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (DraftRealm == null)
            return;

        _ = TrySendSavedMessage(DraftSaveState.Saving);

        if (!Assessment.IsManaged)
            DraftItem = await AssessmentDraft.Upsert(DraftRealm, Assessment, BusinessObject.DisplayName);
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
        if (FamilyNames?.Count > 0 && !value.IsManaged)
            value.FamilyName = FamilyNames[0];

        Influence = value.FactorInfluence ?? new();
        Capacity = value.ProtectiveCapacity ?? new();
        Decisions = value.SafetyDecisions ?? new();
        Factors = value.SafetyFactors ?? new();
        Interventions = value.SafetyInterventions ?? new();

        SelectedChildren.Clear();

        foreach (var child in AvailableChildrenInOutCare)
            if (value.ChildsInOutCare.Contains(child.Id))
                SelectedChildren.Add(child);

        CanDiscard = value.IsManaged;
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
        ClearDecisionBools();
        ClearDecisionUnsafeBools();

        Assessment.PropertyChanged -= Assessment_PropertyChanged;
        Influence.PropertyChanged -= Assessment_PropertyChanged;
        Capacity.PropertyChanged -= Assessment_PropertyChanged;
        Decisions.PropertyChanged -= Assessment_PropertyChanged;
        Factors.PropertyChanged -= Assessment_PropertyChanged;
        Interventions.PropertyChanged -= Assessment_PropertyChanged;
    }

    [RelayCommand]
    public async Task Publish()
    {
#if DEBUG
        WriteSafetyAssessmentJson();
#endif
        var saPublishVm = ServiceProvider.GetService<SafetyAssessmentPublishViewModel>();
        var logger = ServiceProvider.GetService<ILogger<PublishPage>>();

        saPublishVm.BusinessObject = BusinessObject;

        var saPublish = new PublishPage(saPublishVm, logger);
        await Navigator.Navigation.PushAsync(saPublish);
        await Navigator.Navigation.PopModalAsync();
    }

    private static async Task<bool> PromptDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardSafetyAssessmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    [RelayCommand]
    public async Task DiscardDraftAsync()
    {
        if (!await PromptDiscard())
            return;

        UnsubscribeFromAssessment();
        await AssessmentDraft.TryDeleteAsync(Assessment);

        await TrySendSavedMessage(DraftSaveState.None);

        await SetupAssessmentDraft();
        SelectedChildren?.Clear();

        await Navigator.Navigation.PopModalAsync();
        SnackbarHandler.ShowText(LocalizedStrings.DiscardedSafetyAssessmentDraft);
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
                foreach (IcmContact child in e.NewItems?.Cast<IcmContact>() ?? [])
                    Assessment.ChildsInOutCare.Add(child.Id);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IcmContact child in e.OldItems?.Cast<IcmContact>() ?? [])
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

    async void SaveStateHandler_SaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        DraftSaveState = e.State;
    }
}
