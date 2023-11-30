using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.ViewModels;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;

namespace Visitz.Views.Navigation;

public partial class NavRailViewModel : VisitzViewModel
{
    [ObservableProperty]
    public IEnumerable<NavItem> navigationItems;

    [ObservableProperty]
    public NavItem selectedNavItem;

    public override void PageCreated()
    {
        base.PageCreated();

        var items = new List<NavItem>()
        {
            new()
            {
                Text = LocalizedStrings.Caseload,
                ContentViewType = typeof(CaseloadContainerView),
                Color = Colors.White,
                IconSize = 30,
                SelectedImageSource = MaterialIcons.Folder_open.GetFilledMaterialIcon(Colors.White),
                UnselectedImageSource = MaterialIcons.Folder_open.GetUnfilledMaterialIcon(Colors.White),
            }
        };

        if (DebugOptions.Enabled)
            items.Add(new()
            {
                Text = "",
                ContentViewType = typeof(DebugOptionsView),
            });

        NavigationItems = items;

        SelectedNavItem = items.First();
    }
}
