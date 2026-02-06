using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.Navigation;

public partial class EntityNavViewModel : VisitzViewModel,
    IBusinessObjectHolder,
    IRequestedEntitySection,
    IFocusDraftItem
{
    [ObservableProperty]
    public IBusinessObject businessObject;

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
        ContentViewType = typeof(SafetyAssessmentListView),
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

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        BuildNavList();

        SelectedEntityNavItem ??= DefaultNavItem;

        CacheDeletedKeyplayer = BusinessObject.DisplayName;
        CacheDeletedEntityType = BusinessObject.EntityType.GetDisplayString();

        await SetupDraftsObserver();
        BusinessObject.SubscribePropertyChanged(EntityNavViewModel_PropertyChanged);

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, ReceiveEntityNavMessage);
    }

    bool disposed;
    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
            realmQueryMap.Dispose();

            BusinessObject.UnsubscribePropertyChanged(EntityNavViewModel_PropertyChanged);

            StrongReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }

        base.Dispose(disposing);
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
            .Where(draft => draft.ParentEntityId == BusinessObject.FileNumber));

        var attachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        realmQueryMap.Subscribe(attachmentsRealm, attachmentsRealm.All<AttachmentDraft>()
            .Where(draft => draft.RelatedEntityId == BusinessObject.FileNumber));

        if (ShouldShowSafetyAssessment())
        {
            var assessmentRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
            realmQueryMap.Subscribe(assessmentRealm, assessmentRealm.All<AssessmentDraft>()
                .Where(draft => draft.DraftEntityId == BusinessObject.FileNumber));
        }

        if (ShouldShowChildYouthVisits())
        {
            var visitsRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
            realmQueryMap.Subscribe(visitsRealm, visitsRealm.All<PersonVisitDraft>()
                .Where(draft => draft.RelatedEntityId == BusinessObject.Id));
        }
    }

    private void EntityNavViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        IBusinessObject bobj = sender as IBusinessObject;

        if (e.PropertyName == nameof(bobj.IsValid) && !bobj.IsValid)
            _ = EntityUnassignedGoBack();
    }

    private void RealmQueryMap_ItemsChanged(
        object sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
    {
        if (e.Type == typeof(NoteDraft))
            Notes.HasDraft = e.Items.Any();
        else if (e.Type == typeof(AssessmentDraft))
            SafetyAssessment.HasDraft = e.Items.Any();
        else if (e.Type == typeof(AttachmentDraft))
            Attachments.HasDraft = e.Items.Any();
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
            EntitySection.SafetyAssessment or EntitySection.SafetyAssessmentEntry => SafetyAssessment,
            EntitySection.Attachments => Attachments,
            EntitySection.ChildYouthVisits or EntitySection.ChildYouthVisitsEntry => ChildYouthVisits,
            _ => Details,
        };
    }

    [RelayCommand]
    public void EntityNavSelected()
    {
        var msg = new EntityNavMessage(
            SelectedEntityNavItem,
            BusinessObject,
            RequestedSection,
            FocusedDraftItem);

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
        await Navigator.CurrentOpenPage.DisplayAlertAsync(
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
        return BusinessObject.EntityType == EntityType.Incident;
    }

    private bool ShouldShowChildYouthVisits()
    {
        return BusinessObject.EntityType == EntityType.Case
            && BusinessObject.EntitySubtype == EntitySubtype.ChildServices;
    }

    private void ReceiveEntityNavMessage(object recipient, EntityNavMessage message)
    {
        if (SelectedEntityNavItem != message.Value.Item1)
            SelectedEntityNavItem = GetMappedNavItem(message.Value.Item3);
    }

    private bool ShouldShowSupportNetwork()
    {
        return BusinessObject.EntityType != EntityType.Memo;
    }
}
