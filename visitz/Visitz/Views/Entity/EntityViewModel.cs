using Realms;
using Syncfusion.Maui.Toolkit.TabView;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Styles;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity;

public partial class EntityViewModel : IcmRecordViewModel
{
    public EntitySection? RequestedSection { get; set; }

    public IDraftItem? FocusedDraftItem { get; set; }

    readonly ObservableRealmQueryMap _queryMap = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        try
        {
            BuildNavList();

            if (RequestedSection != null)
                SelectedTab = GetMappedNavItem(RequestedSection);

            SelectedTab ??= GetTabByType<EntityDetailsView>();

            await SetupDraftIndicatorObservers();
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            DisposeTabViews();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    async Task SetupDraftIndicatorObservers()
    {
        string fileNumber = BusinessObject.FileNumber;
        _queryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

        if (GetTabByType<EntityNotesView>() != null)
        {
            var noteRealm = await VisitzRealms.GetNoteDraftsRealmAsync();
            _queryMap.Subscribe(
                noteRealm,
                noteRealm.All<NoteDraft>().Where(draft => draft.ParentEntityId == fileNumber)
            );
        }

        if (GetTabByType<AttachmentsView>() != null)
        {
            var attachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
            _queryMap.Subscribe(
                attachmentsRealm,
                attachmentsRealm
                    .All<AttachmentDraft>()
                    .Where(draft =>
                        draft.RelatedEntityId == RowId || draft.RelatedEntityId == BusinessObject.FileNumberBinding
                    )
            );
        }

        if (GetTabByType<SafetyAssessmentListView>() != null)
        {
            var assessmentRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
            _queryMap.Subscribe(
                assessmentRealm,
                assessmentRealm.All<AssessmentDraft>().Where(draft => draft.DraftEntityId == fileNumber)
            );
        }

        if (GetTabByType<ChildYouthVisitListView>() != null)
        {
            var visitsRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
            _queryMap.Subscribe(
                visitsRealm,
                visitsRealm.All<PersonVisitDraft>().Where(draft => draft.RelatedEntityId == RowId)
            );
        }
    }

    void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(NoteDraft))
            TrySetDraftIndicator<EntityNotesView>(e.Items.Any());
        else if (e.Type == typeof(AssessmentDraft))
            TrySetDraftIndicator<SafetyAssessmentListView>(e.Items.Any());
        else if (e.Type == typeof(AttachmentDraft))
            TrySetDraftIndicator<AttachmentsView>(e.Items.Any());
        else if (e.Type == typeof(PersonVisitDraft))
            TrySetDraftIndicator<ChildYouthVisitListView>(e.Items.Any());
    }

    void TrySetDraftIndicator<T>(bool hasDraft)
        where T : BaseContentView
    {
        if (GetTabByType<T>() is not SfTabItem tab)
            return;

#pragma warning disable CS8601 // Possible null reference assignment.
        // SfTabView.ImageSource is not declared nullable even though documentation suggests it should
        // TODO: Remove suppression once fixed in library
        tab.ImageSource = hasDraft
            ? MaterialIcons.GetFilledMaterialIcon(MaterialIcons.Draft, VisitzColors.BC_Gold)
            : null;
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}
