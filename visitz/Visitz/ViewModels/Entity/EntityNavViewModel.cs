using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Views.Entity;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;

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

    public override void Create()
    {
        base.Create();

        EntityNavItems = BuildNavList();

        SelectedEntityNavItem ??= DefaultNavItem;
    }

    public override void Destroy()
    {
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

        if (CaseloadItem.EntityType.ParseEntityType() == EntityType.Incident)
            items.Add(NavItems.SafetyAssessment);

        return items;
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
}
