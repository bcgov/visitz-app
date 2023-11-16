using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.ViewModels;

namespace Visitz.Views.Entity;

public partial class EntityNavViewModel : VisitzViewModel, ICaseloadItemHolder
{
    [ObservableProperty]
    public CaseloadItem caseloadItem;

    [ObservableProperty]
    public NavItem headerNavItem;

    [ObservableProperty]
    public IEnumerable<NavItem> entityNavItems;

    [ObservableProperty]
    public NavItem selectedEntityNavItem;

    public override void PageCreated()
    {
        base.PageCreated();

        EntityNavItems = new List<NavItem>()
        {
            
            new() { Text = LocalizedStrings.Details, ContentViewType = typeof(EntityDetailsView)},
            new() { Text = LocalizedStrings.FamilyMembers, ContentViewType = typeof(EntityContactsView)},
            new() { Text = LocalizedStrings.Notes, ContentViewType = typeof(EntityNotesView)},
        };

        SelectedEntityNavItem = EntityNavItems.First();
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
