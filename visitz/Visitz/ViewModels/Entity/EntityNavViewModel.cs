using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Entity;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.ViewModels.Entity;

public partial class EntityNavViewModel : VisitzViewModel, ICaseloadItemHolder, IRequestedEntitySection
{
    public static class NavItems
    {
        public static readonly EntityNavItem Details = new()
		{
			Text = LocalizedStrings.Details,
			ContentViewType = typeof(EntityDetailsView)
		};
        
        public static readonly EntityNavItem FamilyMembers = new()
		{
			Text = LocalizedStrings.FamilyMembers,
			ContentViewType = typeof(EntityContactsView),
			Section = EntitySection.Family,
		};
        
        public static readonly EntityNavItem Notes = new()
		{
			Text = LocalizedStrings.Notes,
			ContentViewType = typeof(EntityNotesView),
			Section = EntitySection.Notes,
		};

        public static readonly EntityNavItem SafetyAssessment = new()
		{
			Text = LocalizedStrings.SafetyAssessment,
			ContentViewType = typeof(EntitySafetyAssessView),
			Section = EntitySection.SafetyAssessment,
		};
    }

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public EntityNavItem headerNavItem;

    [ObservableProperty]
    public IList<EntityNavItem> entityNavItems;

    [ObservableProperty]
    public EntityNavItem selectedEntityNavItem;

	[ObservableProperty]
	public EntitySection requestedSection;

    public EntityNavItem DefaultNavItem => EntityNavItems?.FirstOrDefault();

	private readonly ObservableRealmQueryMap realmQueryMap = new();

    public override async void Create()
    {
        base.Create();

        EntityNavItems = BuildNavList();

        SelectedEntityNavItem ??= DefaultNavItem;

		await SetupDraftsObserver();
    }

    public override void Destroy()
    {
		realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
		realmQueryMap.Dispose();

        StrongReferenceMessenger.Default.UnregisterAll(this);

        base.Destroy();
    }

    private List<EntityNavItem> BuildNavList()
    {
        var items = new List<EntityNavItem>()
        {
            NavItems.Details,
            NavItems.FamilyMembers,
            NavItems.Notes,
        };

        if (ShouldShowSafetyAssessment())
            items.Add(NavItems.SafetyAssessment);

        return items;
    }

	private async Task SetupDraftsObserver()
	{
		realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

		var noteRealm = await VisitzRealms.GetNoteDraftsRealmAsync();
		realmQueryMap.Subscribe(noteRealm, noteRealm.All<NoteDraft>()
			.Where(draft => draft.ParentEntityId == CaseloadItem.CaseIncidentNumber));

		if (ShouldShowSafetyAssessment())
		{
			var assessmentRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
			realmQueryMap.Subscribe(assessmentRealm, assessmentRealm.All<AssessmentDraft>()
				.Where(draft => draft.DraftEntityId == CaseloadItem.CaseIncidentNumber));
		}
	}

	private void RealmQueryMap_ItemsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
	{
		if (e.Type == typeof(NoteDraft))
			NavItems.Notes.HasDraft = e.Items.Any();
		else if (e.Type == typeof(AssessmentDraft))
			NavItems.SafetyAssessment.HasDraft = e.Items.Any();
	}

	public void SetRequestedSection(EntitySection section)
	{
		RequestedSection = section;

		SelectedEntityNavItem = section switch
		{
			EntitySection.Family => NavItems.FamilyMembers,
			EntitySection.Notes or EntitySection.NoteEntry => NavItems.Notes,
			EntitySection.SafetyAssessment => NavItems.SafetyAssessment,
			_ => NavItems.Details,
		};
	}

	[RelayCommand]
    public void EntityNavSelected()
    {
		var msg = new EntityNavMessage(SelectedEntityNavItem, CaseloadItem, RequestedSection);
		StrongReferenceMessenger.Default.Send(msg);

		RequestedSection = EntitySection.Unknown;
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }

	private bool ShouldShowSafetyAssessment()
	{
		return CaseloadItem.EntityType.ParseEntityType() == EntityType.Incident;
	}
}
