using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.BaseClasses;

public abstract partial class DraftEditViewModel<TDraftItem> : IcmRecordViewModel
    where TDraftItem : IDraftItem, IRealmObject, INotifyPropertyChanged, new()
{
    readonly DraftSaveStateHandler _saveStateHandler = new();

    Realm? _draftRealm;

    [ObservableProperty]
    public partial TDraftItem? Draft { get; set; }

    public virtual Func<TDraftItem, INotifyPropertyChanged?> ObjectToObserveChanges { get; set; } =
        static draft => draft;

    protected INotifyPropertyChanged? ObjectToObserve { get; set; }

    [ObservableProperty]
    public partial bool CanDiscard { get; set; }

    public abstract string DiscardWarningMessage { get; }

    public abstract string DiscardedMessage { get; }

    [ObservableProperty]
    public partial bool AllowPublish { get; set; }

    [ObservableProperty]
    public partial DraftSaveState DraftSaveState { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _draftRealm = await MakeRealm();
        _saveStateHandler.SaveStateChanged += SaveStateHandler_SaveStateChanged;

        SetupDraft(Draft ?? InitNewDraft());
    }

    bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _saveStateHandler.SaveStateChanged -= SaveStateHandler_SaveStateChanged;
            _saveStateHandler.Dispose();

            SetupDraft(default);

            _draftRealm?.Dispose();
            _draftRealm = null;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    protected virtual async Task<Realm> MakeRealm()
    {
        return await VisitzRealms.GetUserDataRealmAsync();
    }

    TDraftItem InitNewDraft()
    {
        var draft = new TDraftItem();
        draft.InitDraftWith(BusinessObject);
        return draft;
    }

    void SetupDraft(TDraftItem? newDraft)
    {
        Draft?.PropertyChanged -= Draft_PropertyChanged;
        ObjectToObserve?.PropertyChanged -= ChangesToObserve_PropertyChanged;

        Draft = newDraft;

        if (Draft != null)
        {
            ObjectToObserve = ObjectToObserveChanges?.Invoke(Draft);
            ObjectToObserve?.PropertyChanged += ChangesToObserve_PropertyChanged;
        }

        UpdateCanDiscard();
        UpdateAllowPublish();
    }

    protected virtual void UpdateCanDiscard()
    {
        CanDiscard = Draft != null && Draft.IsManaged;
    }

    void UpdateAllowPublish()
    {
        AllowPublish = Draft != null && EnablePublish(Draft);
    }

    protected abstract bool EnablePublish(TDraftItem draft);

    private void Draft_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Draft.IsManaged))
            UpdateCanDiscard();
    }

    private void ChangesToObserve_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (
            _draftRealm == null
            || Draft == null
            || e.PropertyName
                is nameof(Draft.LastUpdatedBinding)
                    or nameof(Draft.LastUpdated)
                    or nameof(Draft.IsManaged)
        )
            return;

        if (!Draft.IsManaged)
            _draftRealm.Commit(() => _draftRealm.Add(Draft));

        UpdateCanDiscard();
        UpdateAllowPublish();
        Draft.LastUpdatedBinding = DateTimeOffset.Now;
        _ = _saveStateHandler.Saving();
    }

    private void SaveStateHandler_SaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        DraftSaveState = e.State;
    }

    private static async Task<bool> PromptShouldDiscard(string discardMessage)
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            discardMessage,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    [RelayCommand]
    public async Task DiscardDraftAsync()
    {
        if (!CanDiscard || _draftRealm == null || Draft == null || !await PromptShouldDiscard(DiscardWarningMessage))
            return;

        TDraftItem draft = Draft;

        SetupDraft(default);

        await _draftRealm.CommitAsync(() => _draftRealm.Remove(draft));

        _saveStateHandler.Clear();

        await Navigator.Navigation.PopModalAsync();
        SnackbarHandler.ShowText(DiscardedMessage);
    }

    [RelayCommand]
    public async Task PublishDraftAsync()
    {
        if (AllowPublish)
            await DoPublishDraftAsync();
    }

    protected abstract Task DoPublishDraftAsync();
}
