using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Resources.Localization;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitViewModel : IcmRecordViewModel
{
    public static readonly string VisitTypeGroup = "VisitTypeGroup";
    public static readonly string VisitDetailGroup = "VisitDetailGroup";
    public static readonly int CharacterLimit = 4000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;

    private bool _disposed;

    Realm? DraftRealm { get; set; }

    CaseRecord Case => (CaseRecord)BusinessObject;

    [ObservableProperty]
    public partial PersonVisitDraft? Draft { get; set; }

    [ObservableProperty]
    public partial DateTime MaxDate { get; set; } = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public partial string VisitDescription { get; set; } = "";

    [ObservableProperty]
    public partial DateTimeOffset DateOfVisit { get; set; }

    [ObservableProperty]
    public partial bool IsUpdatingEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool HideElements { get; set; } = true;

    [ObservableProperty]
    public partial bool AllowDiscard { get; set; }

    [ObservableProperty]
    public partial bool AllowPublish { get; set; }

    [ObservableProperty]
    public partial int RemainingCharacters { get; set; } = CharacterLimit;

    [ObservableProperty]
    public partial int CharacterCount { get; set; }

    [ObservableProperty]
    public partial PersonVisit? PersonVisitItem { get; set; }

    [ObservableProperty]
    public partial bool ShowFullForm { get; set; } = true;

    [ObservableProperty]
    public partial GridLength DetailsRowHeight { get; set; } = GridLength.Star;

    [ObservableProperty]
    public partial DraftSaveState DraftSaveState { get; set; }

    public DraftSaveStateHandler SaveStateHandler { get; } = new();

    [ObservableProperty]
    public partial List<VisitDetailListItem> DetailItems { get; set; } = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        DraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();

        if (PersonVisitItem == null && IsUpdatingEnabled)
            Draft = PersonVisitDraft.GetDraft(DraftRealm, Case.Id) ?? new(Case);

        if (PersonVisitItem == null)
        {
            string error = "Unable to load visit record";
            Logger.LogError(error);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(error);
            return;
        }

        DetailItems =
        [
            new(PersonVisitDetails.Api_ExemptionChildDeclined, PersonVisitItem),
            new(PersonVisitDetails.Api_ExemptionOther, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivateInHome, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivatePlanning, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivateRelational, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivateWithCaregiver, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitAge0_5, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitInHome, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitMedicalSupportNeeds, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitNotInHome, PersonVisitItem),
        ];

        AddOtherVisitDetails();

        if (!IsUpdatingEnabled)
            DetailItems = DetailItems.Where(item => item.IsChecked).ToList();

        SaveStateHandler.SaveStateChanged += ViewModel_DraftSaveStateChanged;
        SaveStateHandler.Clear();
        UpdateAllowPublish();
    }

    void AddOtherVisitDetails()
    {
        if (PersonVisitItem == null)
            return;

        var knownDetails = DetailItems.Select(item => item.DetailValue).ToList();
        var otherDetails = PersonVisitItem.VisitDetails.Except(knownDetails);

        foreach (var other in otherDetails)
            DetailItems.Add(new VisitDetailListItem(other, PersonVisitItem));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Draft = null;
            PersonVisitItem = null;

            SaveStateHandler.SaveStateChanged -= ViewModel_DraftSaveStateChanged;
            SaveStateHandler.Dispose();

            DraftRealm?.Dispose();
            DraftRealm = null;

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    private async void ViewModel_DraftSaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        DraftSaveState = e.State;
    }

    partial void OnPersonVisitItemChanged(PersonVisit? oldValue, PersonVisit? newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= PersonVisitItem_PropertyChanged;

        if (newValue != null)
        {
            newValue.PropertyChanged += PersonVisitItem_PropertyChanged;
            UpdateAllowPublish();
        }
    }

    private async void PersonVisitItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!IsUpdatingEnabled)
            return;

        await HandleDraft();

        UpdateAllowPublish();
    }

    [RelayCommand]
    public async Task PublishInPersonVisit()
    {
        if (!AllowPublish)
            return;

        var publishVm = ServiceProvider.GetService<ChildYouthVisitPublishViewModel>();
        var logger = ServiceProvider.GetService<ILogger<PublishPage>>();

        publishVm.BusinessObjectId = BusinessObject.Id;

        await Navigator.Navigation.PopModalAsync();
        await Navigator.Navigation.PushAsync(new PublishPage(publishVm, logger));
    }

    [RelayCommand]
    public async Task DiscardDraft()
    {
        if (!await PromptDiscard())
            return;

        if (Draft != null && DraftRealm != null)
        {
            await DraftRealm.WriteAsync(() => DraftRealm.Remove(Draft));
            Draft = null;
        }

        await Navigator.Navigation.PopModalAsync();
        SnackbarHandler.ShowText(LocalizedStrings.DiscardedVisitDraft);
    }

    private static async Task<bool> PromptDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardVisitDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    private void UpdateAllowPublish()
    {
        AllowPublish =
            PersonVisitItem?.VisitDetails.Count > 0
            && PersonVisitItem?.VisitDescription?.Length > 0
            && CharacterCount <= CharacterLimit;
    }

    TaskCompletionSource? DraftInitTcs;

    private async Task HandleDraft()
    {
        if (Draft == null || DraftRealm == null)
            return;

        if (DraftInitTcs != null)
            await DraftInitTcs.Task;

        if (PersonVisitItem != null && !PersonVisitItem.IsManaged && BusinessObject != null && Case != null)
        {
            DraftInitTcs = new();

            Draft = await PersonVisitDraft.Upsert(DraftRealm, Case.Id, PersonVisitItem, BusinessObject.DisplayName);

            DraftInitTcs.TrySetResult();
        }
        else if (Draft?.IsValid ?? false)
            Draft.LastUpdatedBinding = DateTimeOffset.Now;

        _ = SaveStateHandler.Saving();
    }

    partial void OnCharacterCountChanged(int value)
    {
        RemainingCharacters = CharacterLimit - value;
        UpdateAllowPublish();
    }

    partial void OnDraftChanged(PersonVisitDraft? value)
    {
        PersonVisitItem = value?.Visit;
        AllowDiscard = value?.IsManaged ?? false;
    }

    partial void OnShowFullFormChanged(bool value)
    {
        DetailsRowHeight = value ? GridLength.Star : 0;
    }
}
