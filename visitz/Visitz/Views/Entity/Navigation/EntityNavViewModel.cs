using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.FamilyMembers;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using Visitz.Views.Entity.SupportNetwork;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Interfaces;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.Navigation;

public partial class EntityNavViewModel : VisitzViewModel,
    ICaseloadItemHolder,
    IRequestedEntitySection,
    IFocusDraftItem
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public EntityNavItem headerNavItem;

    [ObservableProperty]
    public ObservableCollection<EntityNavItem> entityNavItems = [];

    [ObservableProperty]
    public EntityNavItem selectedEntityNavItem;

    [ObservableProperty]
    public EntitySection requestedSection;

    public IDraftItem FocusedDraftItem { get; set; }

    public EntityNavItem DefaultNavItem => EntityNavItems?.FirstOrDefault();

    private readonly ObservableRealmQueryMap realmQueryMap = new();

    private readonly EntityNavItem Details = new()
    {
        Text = LocalizedStrings.Details,
        ContentViewType = typeof(EntityDetailsView)
    };

    private readonly EntityNavItem FamilyMembers = new()
    {
        Text = LocalizedStrings.FamilyMembers,
        ContentViewType = typeof(EntityContactsView),
        Section = EntitySection.Family,
    };

    private readonly EntityNavItem Notes = new()
    {
        Text = LocalizedStrings.Notes,
        ContentViewType = typeof(EntityNotesView),
        Section = EntitySection.Notes,
    };

    private readonly EntityNavItem Attachments = new()
    {
        Text = LocalizedStrings.Attachments,
        ContentViewType = typeof(AttachmentsView),
        Section = EntitySection.Attachments,
    };

    private readonly EntityNavItem SafetyAssessment = new()
    {
        Text = LocalizedStrings.SafetyAssessment,
        ContentViewType = typeof(EntitySafetyAssessView),
        Section = EntitySection.SafetyAssessment,
    };

    private readonly EntityNavItem ChildYouthVisits = new()
    {
        Text = LocalizedStrings.ChildYouthVisits,
        ContentViewType = typeof(ChildYouthVisitListView),
        Section = EntitySection.ChildYouthVisits,
    };

    private readonly EntityNavItem SupportNetwork = new()
    {
        Text = LocalizedStrings.SupportNetwork,
        ContentViewType = typeof(SupportNetworkListView),
        Section = EntitySection.SupportNetwork,
    };

    private string CacheDeletedKeyplayer;
    private string CacheDeletedEntityType;

    public override async void Create()
    {
        base.Create();

        BuildNavList();

        SelectedEntityNavItem ??= DefaultNavItem;

        CacheDeletedKeyplayer = CaseloadItem.DisplayName;
        CacheDeletedEntityType = CaseloadItem.EntityType;

        await SetupDraftsObserver();

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, ReceiveEntityNavMessage);
    }

    public override void Destroy()
    {
        realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
        realmQueryMap.Dispose();

        StrongReferenceMessenger.Default.UnregisterAll(this);

        base.Destroy();
    }

    private void BuildNavList()
    {
        EntityNavItems.Add(Details);
        EntityNavItems.Add(FamilyMembers);
        EntityNavItems.Add(Notes);
        EntityNavItems.Add(Attachments);

        if (ShouldShowSafetyAssessment())
            EntityNavItems.Add(SafetyAssessment);

        if (ShouldShowChildYouthVisits())
            EntityNavItems.Add(ChildYouthVisits);

        if (ShouldShowSupportNetwork())
            EntityNavItems.Add(SupportNetwork);
    }

    private async Task SetupDraftsObserver()
    {
        realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

        var noteRealm = await VisitzRealms.GetNoteDraftsRealmAsync();
        realmQueryMap.Subscribe(noteRealm, noteRealm.All<NoteDraft>()
            .Where(draft => draft.ParentEntityId == CaseloadItem.CaseIncidentNumber));

        var attachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        realmQueryMap.Subscribe(attachmentsRealm, attachmentsRealm.All<AttachmentDraft>()
            .Where(draft => draft.RelatedEntityId == CaseloadItem.CaseIncidentNumber));

        var caseloadRealm = await VisitzRealms.GetIcmDataRealmAsync();
        realmQueryMap.Subscribe(caseloadRealm, caseloadRealm.All<CaseloadItem>()
            .Where(item => item.CaseIncidentNumber == CaseloadItem.CaseIncidentNumber));

        if (ShouldShowSafetyAssessment())
        {
            var assessmentRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
            realmQueryMap.Subscribe(assessmentRealm, assessmentRealm.All<AssessmentDraft>()
                .Where(draft => draft.DraftEntityId == CaseloadItem.CaseIncidentNumber));
        }

        if (ShouldShowChildYouthVisits())
        {
            var visitsRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
            realmQueryMap.Subscribe(visitsRealm, visitsRealm.All<PersonVisitDraft>()
                .Where(draft => draft.RelatedEntityId == CaseloadItem.RowId));
        }
    }

    private void RealmQueryMap_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(NoteDraft))
            Notes.HasDraft = e.Items.Any();
        else if (e.Type == typeof(AssessmentDraft))
            SafetyAssessment.HasDraft = e.Items.Any();
        else if (e.Type == typeof(AttachmentDraft))
            Attachments.HasDraft = e.Items.Any();
        else if (e.Type == typeof(CaseloadItem) && e.Changes?.DeletedIndices?.Length > 0)
            _ = EntityUnassignedGoBack();
        else if (e.Type == typeof(PersonVisitDraft))
            ChildYouthVisits.HasDraft = e.Items.Any();
    }

    public void SetRequestedSection(EntitySection section, IDraftItem focusedDraftItem = null)
    {
        RequestedSection = section;
        FocusedDraftItem = focusedDraftItem;

        SelectedEntityNavItem = GetMappedNavItem(section);
    }

    private EntityNavItem GetMappedNavItem(EntitySection? section)
    {
        return section switch
        {
            EntitySection.Family => FamilyMembers,
            EntitySection.Notes or EntitySection.NoteEntry => Notes,
            EntitySection.SafetyAssessment => SafetyAssessment,
            EntitySection.Attachments => Attachments,
            EntitySection.ChildYouthVisits or EntitySection.ChildYouthVisitsEntry => ChildYouthVisits,
            _ => Details,
        };
    }

    [RelayCommand]
    public void EntityNavSelected()
    {
        var msg = new EntityNavMessage(SelectedEntityNavItem, CaseloadItem, RequestedSection, FocusedDraftItem);
        StrongReferenceMessenger.Default.Send(msg);

        RequestedSection = EntitySection.Unknown;
        FocusedDraftItem = null;
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }

    private async Task EntityUnassignedGoBack()
    {
        GoBack();
        await Navigator.CurrentOpenPage.DisplayAlert(
            string.Format(
                LocalizedStrings.RecordRemovedFromCaseload,
                CacheDeletedEntityType,
                CacheDeletedKeyplayer),
            string.Format(
                LocalizedStrings.RecordRemovedFromCaseloadDetails,
                CacheDeletedEntityType,
                CacheDeletedKeyplayer),
            LocalizedStrings.Ok
        );
    }

    private bool ShouldShowSafetyAssessment()
    {
        return CaseloadItem.EntityType.ParseEntityType() == EntityType.Incident;
    }

    private bool ShouldShowChildYouthVisits()
    {
        return CaseloadItem.EntityType.ParseEntityType() == EntityType.Case
            && CaseloadItem.CaseIncidentType.ParseEntitySubtype() == EntitySubtype.ChildServices;
    }

    private void ReceiveEntityNavMessage(object recipient, EntityNavMessage message)
    {
        if (SelectedEntityNavItem != message.Value.Item1)
            SelectedEntityNavItem = GetMappedNavItem(message.Value.Item3);
    }

    private bool ShouldShowSupportNetwork()
    {
        return CaseloadItem.EntityType.ParseEntityType() != EntityType.Memo;
    }
}
