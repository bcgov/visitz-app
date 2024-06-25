using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.ViewModels;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.FamilyMembers;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.Navigation;

public partial class EntityNavViewModel : VisitzViewModel, ICaseloadItemHolder, IRequestedEntitySection
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

	private readonly EntityNavItem SafetyAssessment = new()
	{
		Text = LocalizedStrings.SafetyAssessment,
		ContentViewType = typeof(EntitySafetyAssessView),
		Section = EntitySection.SafetyAssessment,
	};

	public override async void Create()
    {
        base.Create();

        BuildNavList();

        SelectedEntityNavItem ??= DefaultNavItem;

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

        if (ShouldShowSafetyAssessment())
            EntityNavItems.Add(SafetyAssessment);
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
			Notes.HasDraft = e.Items.Any();
		else if (e.Type == typeof(AssessmentDraft))
			SafetyAssessment.HasDraft = e.Items.Any();
	}

	public void SetRequestedSection(EntitySection section)
	{
		RequestedSection = section;

		SelectedEntityNavItem = GetMappedNavItem(section);
	}

	private EntityNavItem GetMappedNavItem(EntitySection? section)
	{
		return section switch
		{
			EntitySection.Family => FamilyMembers,
			EntitySection.Notes or EntitySection.NoteEntry => Notes,
			EntitySection.SafetyAssessment => SafetyAssessment,
			_ => Details,
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

	private void ReceiveEntityNavMessage(object recipient, EntityNavMessage message)
	{
		if (SelectedEntityNavItem != message.Value.Item1)
			SelectedEntityNavItem = GetMappedNavItem(message.Value.Item3);
	}
}
