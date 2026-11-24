using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Visitz.Controls.ActionButton;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Drafts;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.ChildYouthInfo;

public partial class ChildYouthInfoListViewModel : IcmRecordViewModel, IOpenFocusDraftItem
{
    readonly bool _ascending = false;

    bool _disposed;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    ObservableQuery<ContactEducation, EducationListItemViewModel>? _educationQuery;
    ObservableQuery<ContactEducationDraft, EducationListItemViewModel>? _educationDraftsQuery;

    ObservableQuery<ContactMedicalBehavioral, MedicalBehavioralListItemViewModel>? _medicalBehavioralQuery;
    ObservableQuery<ContactMedicalBehavioralDraft, MedicalBehavioralListItemViewModel>? _medicalBehavioralDraftsQuery;

    ObservableQuery<ContactLanguage, LanguagesListItemViewModel>? _languagesQuery;
    ObservableQuery<ContactLanguageDraft, LanguagesListItemViewModel>? _languagesDraftsQuery;

    [ObservableProperty]
    public partial ObservableCollection<EducationListItemViewModel> EducationItems { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<MedicalBehavioralListItemViewModel> MedicalItems { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<LanguagesListItemViewModel> LanguagesItems { get; set; } = [];

    [ObservableProperty]
    public partial List<ActionButtonOption> FabOptions { get; private set; } = [];

    public IDraftItem? FocusedDraftItem { get; set; }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        IsLoading = true;

        FabOptions =
        [
            new ActionButtonOption()
            {
                Text = LocalizedStrings.Education,
                Command = OpenEducationDraftCommand,
                Icon = MaterialIcons.School.GetUnfilledMaterialIcon(),
            },
            new ActionButtonOption()
            {
                Text = LocalizedStrings.MedicalBehavioural,
                Command = OpenMedicalBehavioralDraftCommand,
                Icon = MaterialIcons.Monitor_heart.GetUnfilledMaterialIcon(),
            },
            new ActionButtonOption()
            {
                Text = LocalizedStrings.Languages,
                Command = OpenLanguageDraftCommand,
                Icon = MaterialIcons.Globe.GetUnfilledMaterialIcon(),
            },
        ];

        if (BusinessObject.GetKeyPlayer() is IcmContact keyPlayer)
        {
            await Task.WhenAll(InitReadQueries(keyPlayer), InitDraftQueries());
        }
        else
        {
            string error = $"Unable to find key player for {BusinessObject.DisplayName}";
            Logger.LogError(error);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(error);
        }

        IsLoading = false;

        await OpenDraftAsync();
    }

    async Task InitReadQueries(IcmContact keyPlayer)
    {
        if (DataRealm == null)
            return;

        _educationQuery = new(
            DataRealm,
            query: ContactEducation.GetAllByParent(DataRealm, keyPlayer.Id),
            targetList: EducationItems,
            addItem: (addItem, targetList) => targetList.InsertSorted(new(default, addItem), ascending: _ascending),
            removeItem: (removeItem, targetList) =>
                targetList.RemoveWrappedItem(removeItem, unwrapper => unwrapper.ReadItem)
        );

        _medicalBehavioralQuery = new(
            DataRealm,
            query: ContactMedicalBehavioral.GetAllByParent(DataRealm, keyPlayer.Id),
            targetList: MedicalItems,
            addItem: (addItem, targetList) => targetList.InsertSorted(new(default, addItem), ascending: _ascending),
            removeItem: (removeItem, targetList) =>
                targetList.RemoveWrappedItem(removeItem, unwrapper => unwrapper.ReadItem)
        );

        _languagesQuery = new(
            DataRealm,
            query: ContactLanguage.GetAllByParent(DataRealm, keyPlayer.Id),
            targetList: LanguagesItems,
            addItem: (addItem, targetList) => targetList.InsertSorted(new(default, addItem), ascending: _ascending),
            removeItem: (removeItem, targetList) =>
                targetList.RemoveWrappedItem(removeItem, unwrapper => unwrapper.ReadItem)
        );

        await Task.WhenAll(_educationQuery.Loaded, _medicalBehavioralQuery.Loaded, _languagesQuery.Loaded);
    }

    async Task InitDraftQueries()
    {
        var userDataRealm = await VisitzRealms.GetUserDataRealmAsync();

        _educationDraftsQuery = new(
            userDataRealm,
            query: ContactEducationDraft.GetAllByRelated(userDataRealm, EntityType, RowId),
            targetList: EducationItems,
            addItem: (addDraft, targetList) =>
                targetList.InsertSorted(new(addDraft, addDraft.ContactEducation), ascending: _ascending),
            removeItem: (removeItem, targetList) =>
                targetList.RemoveWrappedItem(removeItem, unwrapper => unwrapper.DraftItem)
        );

        _medicalBehavioralDraftsQuery = new(
            userDataRealm,
            query: ContactMedicalBehavioralDraft.GetAllByRelated(userDataRealm, EntityType, RowId),
            MedicalItems,
            addItem: (addDraft, targetList) =>
                targetList.InsertSorted(new(addDraft, addDraft.ContactMedicalBehavioral), ascending: _ascending),
            removeItem: (removeItem, targetList) =>
                targetList.RemoveWrappedItem(removeItem, unwrapper => unwrapper.DraftItem)
        );

        _languagesDraftsQuery = new(
            userDataRealm,
            query: ContactLanguageDraft.GetAllByRelated(userDataRealm, EntityType, RowId),
            LanguagesItems,
            addItem: (addDraft, targetList) =>
                targetList.InsertSorted(new(addDraft, addDraft.ContactLanguage), ascending: _ascending),
            removeItem: (removeItem, targetList) =>
                targetList.RemoveWrappedItem(removeItem, unwrapper => unwrapper.DraftItem)
        );

        await Task.WhenAll(
            _educationDraftsQuery.Loaded,
            _medicalBehavioralDraftsQuery.Loaded,
            _languagesDraftsQuery.Loaded
        );
    }

    [RelayCommand]
    public async Task OpenEducationDraft(ContactEducationDraft? draft)
    {
        ContactEducationEditView view = ServiceProvider.GetService<ContactEducationEditView>();
        view.Init(RowId, EntityType, draft);

        await Navigator.Navigation.PushModalAsync(view.WrapPageForModal());
    }

    [RelayCommand]
    public async Task OpenMedicalBehavioralDraft(ContactMedicalBehavioralDraft? draft)
    {
        ContactMedicalBehavioralEditView view = ServiceProvider.GetService<ContactMedicalBehavioralEditView>();
        view.Init(RowId, EntityType, draft);

        await Navigator.Navigation.PushModalAsync(view.WrapPageForModal());
    }

    [RelayCommand]
    public async Task OpenLanguageDraft(ContactLanguageDraft? draft)
    {
        ContactLanguageEditView view = ServiceProvider.GetService<ContactLanguageEditView>();
        view.Init(RowId, EntityType, draft);

        await Navigator.Navigation.PushModalAsync(view.WrapPageForModal());
    }

    [RelayCommand]
    public static async Task DiscardEducationDraft(ContactEducationDraft draft)
    {
        string message = string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactEducation.ToLowerInvariant()
        );

        if (await PromptShouldDiscard(message))
            draft.Delete();
    }

    [RelayCommand]
    public static async Task DiscardMedicalBehavioralDraft(ContactMedicalBehavioralDraft draft)
    {
        string message = string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactMedicalBehavioral.ToLowerInvariant()
        );

        if (await PromptShouldDiscard(message))
            draft.Delete();
    }

    [RelayCommand]
    public static async Task DiscardLanguageDraft(ContactLanguageDraft draft)
    {
        string message = string.Format(
            LocalizedStrings.DiscardThisDraftDescription,
            LocalizedStrings.ContactLanguages.ToLowerInvariant()
        );

        if (await PromptShouldDiscard(message))
            draft.Delete();
    }

    protected static async Task<bool> PromptShouldDiscard(string discardMessage)
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            discardMessage,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    [RelayCommand]
    public async Task PublishDraft(IDraftItem draft)
    {
        await PublishPage.Make<ChildYouthInfoPublishViewModel>(vm => vm.SetPayload(BusinessObject, draft));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _educationQuery?.Dispose();
            _educationDraftsQuery?.Dispose();
            _medicalBehavioralQuery?.Dispose();
            _medicalBehavioralDraftsQuery?.Dispose();
            _languagesQuery?.Dispose();
            _languagesDraftsQuery?.Dispose();
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public async Task OpenDraftAsync()
    {
        if (FocusedDraftItem is ContactEducationDraft education)
            await OpenEducationDraft(education);
        else if (FocusedDraftItem is ContactMedicalBehavioralDraft medical)
            await OpenMedicalBehavioralDraft(medical);
        else if (FocusedDraftItem is ContactLanguageDraft language)
            await OpenLanguageDraft(language);
    }
}
