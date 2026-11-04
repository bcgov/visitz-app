using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Controls;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

public partial class DraftsListViewModel : VisitzViewModel
{
    bool _disposed;

    [ObservableProperty]
    public partial bool ShowEmptyView { get; set; }

    ObservableQuery<AssessmentDraft, DraftListItemViewModel>? _assessmentDrafts;

    ObservableQuery<AttachmentDraft, DraftListItemViewModel>? _attachmentDrafts;

    ObservableQuery<NoteDraft, DraftListItemViewModel>? _noteDrafts;

    ObservableQuery<PersonVisitDraft, DraftListItemViewModel>? _visitDrafts;

    [ObservableProperty]
    public partial ObservableCollection<DraftListItemViewModel> AllDraftItems { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<DraftListItemViewModel> FilteredItems { get; set; } = [];

    Realm? DataRealm { get; set; }

    EntitySection SectionToOpen { get; set; }

    public event EventHandler<IDraftItem>? SelectedItemRelatedMissing;

    [ObservableProperty]
    public partial FilterOption<IDraftItem> SelectedFilter { get; set; } = new(LocalizedStrings.AllTypes, _ => true);

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        DataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        await SubscribeForDrafts();
    }

    async Task SubscribeForDrafts()
    {
        AllDraftItems.CollectionChanged += AllDraftItems_CollectionChanged;

        Task<Realm> assessmentsTask = VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        Task<Realm> attachmentsTask = VisitzRealms.GetAttachmentDraftsRealmAsync();
        Task<Realm> notesTask = VisitzRealms.GetNoteDraftsRealmAsync();
        Task<Realm> visitsTask = VisitzRealms.GetPersonVisitDraftsRealmAsync();

        Realm assessmentsRealm = await assessmentsTask;
        Realm attachmentsRealm = await attachmentsTask;
        Realm notesRealm = await notesTask;
        Realm visitsRealm = await visitsTask;

        _assessmentDrafts = new(
            assessmentsRealm,
            assessmentsRealm.All<AssessmentDraft>(),
            AllDraftItems,
            AddDraftItem,
            RemoveDraftItem,
            ItemsChanged
        );
        _attachmentDrafts = new(
            attachmentsRealm,
            attachmentsRealm.All<AttachmentDraft>(),
            AllDraftItems,
            AddDraftItem,
            RemoveDraftItem,
            ItemsChanged
        );
        _noteDrafts = new(
            notesRealm,
            notesRealm.All<NoteDraft>(),
            AllDraftItems,
            AddDraftItem,
            RemoveDraftItem,
            ItemsChanged
        );
        _visitDrafts = new(
            visitsRealm,
            visitsRealm.All<PersonVisitDraft>(),
            AllDraftItems,
            AddDraftItem,
            RemoveDraftItem,
            ItemsChanged
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            _assessmentDrafts?.Dispose();
            _attachmentDrafts?.Dispose();
            _noteDrafts?.Dispose();
            _visitDrafts?.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    static void AddDraftItem(IDraftItem item, ObservableCollection<DraftListItemViewModel> drafts)
    {
        DraftListItemViewModel vm = new(item);
        vm.StartInitAsync();
        drafts.Add(vm);
    }

    static void RemoveDraftItem(IDraftItem item, ObservableCollection<DraftListItemViewModel> drafts)
    {
        if (drafts.FirstOrDefault(draftVm => draftVm.Draft == item) is DraftListItemViewModel foundVm)
        {
            drafts.Remove(foundVm);
            foundVm.Dispose();
        }
    }

    void ItemsChanged(ObservableCollection<DraftListItemViewModel> _, bool hasModifications)
    {
        if (hasModifications)
            FilteredItems.Sort(ascending: false);
    }

    private void AllDraftItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var draft in e.NewItems.Cast<DraftListItemViewModel>())
            {
                if (SelectedFilter.WherePredicate(draft.Draft))
                    FilteredItems.InsertSorted(draft, ascending: false);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var draft in e.OldItems.Cast<DraftListItemViewModel>())
                FilteredItems.Remove(draft);
        }

        ShowEmptyView = !AllDraftItems.Any();
    }

    [RelayCommand]
    private async Task DraftItemSelected(IDraftItem draftItem)
    {
        if (DataRealm == null)
        {
            Logger.LogWarning("Realm unexpectedly null");
            return;
        }

        Type type = draftItem.GetType();
        if (type == typeof(NoteDraft))
            SectionToOpen = EntitySection.NoteEntry;
        else if (type == typeof(AssessmentDraft))
            SectionToOpen = EntitySection.SafetyAssessmentEntry;
        else if (type == typeof(AttachmentDraft))
            SectionToOpen = EntitySection.Attachments;
        else if (type == typeof(PersonVisitDraft))
            SectionToOpen = EntitySection.ChildYouthVisitsEntry;

        if (draftItem.GetRelatedBusinessObjectFrom(DataRealm) is IBusinessObject bobj)
            await MarkForDownloadAndTryOpen(bobj, draftItem);
        else
            SelectedItemRelatedMissing?.Invoke(this, draftItem);
    }

    private async Task MarkForDownloadAndTryOpen(IBusinessObject bobj, IDraftItem draftItem)
    {
        bool shouldDownload = bobj.LocalState?.ShouldDownloadDuringRefresh ?? true;
        bool markForDownload = !shouldDownload;

        if (markForDownload)
        {
            if (await bobj.PromptCanDownloadDependentData())
            {
                try
                {
                    await bobj.DownloadDependentData();
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Logger.LogException(ex, ex.Message);
                    await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
                }
            }
            // else: cancel
        }
        else
            NavigateTo(bobj, SectionToOpen, draftItem);
    }

    static void NavigateTo(IBusinessObject businessObject, EntitySection section, IDraftItem draftItem)
    {
        var caseloadNav = new BusinessObjectSelectedMessage(businessObject, section, draftItem);
        StrongReferenceMessenger.Default.Send(caseloadNav);
    }

    public async Task DeleteDraftAsync(IDraftItem draft)
    {
        var realm = draft.Realm;

        if (realm == null)
        {
            Logger.LogWarning("Realm unexpectedly null");
            return;
        }

        await realm.WriteAsync(async () =>
        {
            if (draft is AssessmentDraft)
            {
                var assessment = SafetyAssessment.FindByIncidentNumber(realm, draft.RelatedEntityId);

                if (assessment != null)
                    realm.Remove(assessment);

                realm.Remove(draft);
            }
            else if (draft is AttachmentDraft attachmentDraft)
                await (attachmentDraft?.DeleteAsync(deleteAttachment: true) ?? Task.CompletedTask);
            else
                realm.Remove(draft);
        });
    }

    partial void OnSelectedFilterChanged(FilterOption<IDraftItem> value)
    {
        FilteredItems = new(AllDraftItems.Where(draftVm => value.WherePredicate(draftVm.Draft)).OrderDescending());
    }
}
