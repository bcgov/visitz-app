using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.ViewModels;
using VisitzModel.Messaging;
using VisitzModel.Models;

namespace Visitz.Views.Entity;

public partial class EntityNavViewModel : VisitzViewModel, ICaseloadItemHolder
{
    public readonly struct NavItems
    {
        public static readonly NavItem Details = new() 
            { Text = LocalizedStrings.Details, ContentViewType = typeof(EntityDetailsView) };
        
        public static readonly NavItem FamilyMembers = new() 
            { Text = LocalizedStrings.FamilyMembers, ContentViewType = typeof(EntityContactsView) };
        
        public static readonly NavItem Notes = new() 
            { Text = LocalizedStrings.Notes, ContentViewType = typeof(EntityNotesView) };

        public static readonly NavItem SafetyAssessment = new()
            { Text = LocalizedStrings.SafetyAssessment, ContentViewType = typeof(EntitySafetyAssessView) };
    }

    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public NavItem headerNavItem;

    [ObservableProperty]
    public IList<NavItem> entityNavItems;

    [ObservableProperty]
    public NavItem selectedEntityNavItem;

    public NavItem DefaultNavItem => EntityNavItems?.FirstOrDefault();

    public override void Create()
    {
        base.Create();

        EntityNavItems = BuildNavList();

        SelectedEntityNavItem = DefaultNavItem;

        StrongReferenceMessenger.Default.Register<EntityNavMessage>(this, (recipient, navMessage) =>
        {
            var (navItem, caseloadItem) = navMessage.Value;

            if (navItem != null)
                (recipient as EntityNavViewModel).SelectedEntityNavItem = navItem;
        });
    }

    public override void Destroy()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);

        base.Destroy();
    }

    private List<NavItem> BuildNavList()
    {
        var items = new List<NavItem>()
        {
            NavItems.Details,
            NavItems.FamilyMembers,
            NavItems.Notes,
        };

        if (CaseloadItem.EntityType.Equals(IcmEntity.Incident))
            items.Add(NavItems.SafetyAssessment);

        return items;
    }

    [RelayCommand]
    public void EntityNavSelected()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavMessage(SelectedEntityNavItem, CaseloadItem));
    }

    [RelayCommand]
    public static void GoBack()
    {
        StrongReferenceMessenger.Default.Send(new EntityNavBackMessage());
    }
}
