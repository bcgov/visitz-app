using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ContactEducationEditViewModel : IcmRecordViewModel
{
    readonly DraftSaveStateHandler _saveStateHandler = new();

    Realm? _userDataRealm;

    public ContactEducation? Education { get; set; }

    [ObservableProperty]
    public partial ContactEducationDraft? Draft { get; set; }

    [ObservableProperty]
    public partial bool CanDiscard { get; set; }

    [ObservableProperty]
    public partial bool AllowPublish { get; set; }

    [ObservableProperty]
    public partial DraftSaveState DraftSaveState { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        _userDataRealm = await VisitzRealms.GetUserDataRealmAsync();
        _saveStateHandler.SaveStateChanged += SaveStateHandler_SaveStateChanged;

        SetupDraft(Draft ?? new(BusinessObject));
    }

    bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _saveStateHandler.SaveStateChanged -= SaveStateHandler_SaveStateChanged;
            _saveStateHandler.Dispose();

            SetupDraft(null);

            _userDataRealm?.Dispose();
            _userDataRealm = null;

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    void SetupDraft(ContactEducationDraft? newDraft)
    {
        if (Draft != null)
        {
            Draft.PropertyChanged -= Draft_PropertyChanged;
            Education?.PropertyChanged -= ContactEducation_PropertyChanged;
        }

        if (newDraft != null)
        {
            Draft = newDraft;
            UpdateCanDiscard();
            Draft.PropertyChanged += Draft_PropertyChanged;

            Education = Draft?.ContactEducation;
            UpdateAllowPublish();
            Education?.PropertyChanged += ContactEducation_PropertyChanged;
        }
        else
        {
            Education?.PropertyChanged -= ContactEducation_PropertyChanged;
            Education = null;

            Draft?.PropertyChanged -= Draft_PropertyChanged;
            Draft = null;
        }
    }

    void UpdateCanDiscard()
    {
        CanDiscard = Draft != null && Draft.IsManaged;
    }

    void UpdateAllowPublish()
    {
        // TODO: proper validation
        AllowPublish = Education != null && Education.CommentsBinding.Length > 0;
    }

    private void Draft_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Draft.IsManaged))
            UpdateCanDiscard();
    }

    private void ContactEducation_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_userDataRealm == null || Draft == null)
            return;

        if (!Draft.IsManaged)
            _userDataRealm.Commit(() => _userDataRealm.Add(Draft));

        UpdateCanDiscard();
        UpdateAllowPublish();
        Draft.LastUpdatedBinding = DateTimeOffset.Now;
        _ = _saveStateHandler.Saving();
    }

    private void SaveStateHandler_SaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        DraftSaveState = e.State;
    }

    private static async Task<bool> PromptShouldDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardContactEducationDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    [RelayCommand]
    public async Task DiscardDraftAsync()
    {
        if (!CanDiscard || _userDataRealm == null || Draft == null || !await PromptShouldDiscard())
            return;

        ContactEducationDraft draft = Draft;
        ContactEducation? education = Education;

        SetupDraft(new(BusinessObject));

        await _userDataRealm.CommitAsync(() =>
        {
            _userDataRealm.Remove(draft);

            if (education != null)
                _userDataRealm.Remove(education);
        });

        _saveStateHandler.Clear();

        await Navigator.Navigation.PopModalAsync();
        SnackbarHandler.ShowText(LocalizedStrings.DiscardedContactEducationDraft);
    }

    [RelayCommand]
    public async Task PublishDraftAsync()
    {
        if (!AllowPublish)
            return;

        // TODO: run publish service
    }
}
