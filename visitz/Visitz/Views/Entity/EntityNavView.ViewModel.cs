using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.ViewModels;

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

    public override void PageCreated()
    {
        base.PageCreated();

        EntityNavItems = BuildNavList();

        SelectedEntityNavItem = DefaultNavItem;
    }

    private List<NavItem> BuildNavList()
    {
        var items = new List<NavItem>()
        {
            NavItems.Details,
            NavItems.FamilyMembers,
            NavItems.Notes,
        };

        // TODO: Remove this ShowSafetyAssessment check when it's fully implemented
        if (CaseloadItem.EntityType.Equals(IcmEntity.Incident) && DebugOptions.ShowSafetyAssessment)
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
