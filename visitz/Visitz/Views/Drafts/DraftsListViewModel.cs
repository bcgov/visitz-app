using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Extensions;
using Visitz.Services.Base;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Interfaces;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Drafts;

internal partial class DraftsListViewModel : VisitzViewModel
{
    bool _disposed;

    [ObservableProperty]
    public ObservableCollection<IDraftItem> draftItems = [];

    readonly ObservableRealmQueryMap queryMap = new();

    Realm DataRealm { get; set; }

    EntitySection SectionToOpen { get; set; }

    public event EventHandler<IDraftItem> SelectedItemRelatedMissing;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        StrongReferenceMessenger.Default.Register<DraftMasterSelectedMessage>(this, DraftMasterSelected);

        DataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        queryMap.ItemsChanged += QueryMap_ItemsChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            queryMap.ItemsChanged -= QueryMap_ItemsChanged;
            queryMap.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }

#pragma warning disable SS001 // Async methods should return a Task to make them awaitable
    // Ignoring SS001 because this function is used like an EventHandler
    private async void DraftMasterSelected(object _, DraftMasterSelectedMessage message)
    {
        await InitTask;

        queryMap.UnsubscribeAll();

        var (type, realm) = message.Value;

        if (type == typeof(NoteDraft))
        {
            SortAndSubscribe(realm, realm.All<NoteDraft>());
            SectionToOpen = EntitySection.NoteEntry;
        }
        else if (type == typeof(AssessmentDraft))
        {
            SortAndSubscribe(realm, realm.All<AssessmentDraft>());
            SectionToOpen = EntitySection.SafetyAssessmentEntry;
        }
        else if (type == typeof(AttachmentDraft))
        {
            SortAndSubscribe(realm, realm.All<AttachmentDraft>());
            SectionToOpen = EntitySection.Attachments;
        }
        else if (type == typeof(PersonVisitDraft))
        {
            SortAndSubscribe(realm, realm.All<PersonVisitDraft>());
            SectionToOpen = EntitySection.ChildYouthVisitsEntry;
        }
        else
            throw new InvalidOperationException($"Type {type} not supported in Drafts view.");
    }
#pragma warning restore SS001 // Async methods should return a Task to make them awaitable

    private void SortAndSubscribe<T>(Realm realm, IQueryable<T> query) where T : IRealmObject
    {
        var sortedQuery = query.Filter($"TRUEPREDICATE SORT({nameof(IDraftItem.LastUpdated)} DESC)");
        queryMap.Subscribe(realm, sortedQuery);
    }

    private void QueryMap_ItemsChanged(
        object _,
        (Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        foreach (var draft in DraftItems)
            draft.Dispose();

        DraftItems.Clear();

        foreach (var item in e.Items)
        {
            IDraftItem draftItem = (IDraftItem)item;
            draftItem.SubscribeRelatedState(DataRealm);

            DraftItems.Add(draftItem);
        }
    }

    [RelayCommand]
    private async Task DraftItemSelected(IDraftItem draftItem)
    {
        if (draftItem.GetRelatedBusinessObjectFrom(DataRealm) is IBusinessObject bobj)
            await MarkForDownloadAndTryOpen(bobj, draftItem);
        else
            SelectedItemRelatedMissing?.Invoke(this, draftItem);
    }

    private async Task MarkForDownloadAndTryOpen(IBusinessObject bobj, IDraftItem draftItem)
    {
        bool markForDownload = !bobj.LocalState.ShouldDownloadDuringRefresh;

        if (markForDownload)
        {
            if (await bobj.PromptCanDownloadDependentData())
            {
                try
                {
                    var result = await bobj.DownloadDependentData();

                    if (result == VisitzService.Result.Successful)
                        NavigateTo(bobj, SectionToOpen, draftItem);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
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

        var appNav = new AppNavMessage(new() { ContentViewType = typeof(CaseloadContainerView) });
        StrongReferenceMessenger.Default.Send(appNav);
    }

    public static async Task DeleteDraft(IDraftItem draft)
    {
        var realm = draft.Realm;

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
                await attachmentDraft.Attachment.DeleteAsync();
            else
                realm.Remove(draft);
        });
    }
}
