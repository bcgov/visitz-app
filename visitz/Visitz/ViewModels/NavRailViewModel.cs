using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.FontIcons;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;
using Visitz.Views.Drafts;
using VisitzModel.Messaging;
using VisitzModel.Models;

namespace Visitz.ViewModels;

public partial class NavRailViewModel : VisitzViewModel
{
    [ObservableProperty]
    public IEnumerable<NavItem> navigationItems;

    [ObservableProperty]
    public NavItem selectedNavItem;

    public override void Create()
    {
        base.Create();

        NavigationItems = BuildNavItems();
        SelectedNavItem = NavigationItems.First();
    }

    private static List<NavItem> BuildNavItems()
    {
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
            },
			new()
			{
				Text = LocalizedStrings.Drafts,
				ContentViewType = typeof(DraftsContainerView),
				Color = Colors.White,
				IconSize = 30,
				SelectedImageSource = MaterialIcons.Draft.GetFilledMaterialIcon(Colors.White),
				UnselectedImageSource = MaterialIcons.Draft.GetUnfilledMaterialIcon(Colors.White),
			},
		};

        if (DebugOptions.Enabled)
            items.Add(new()
            {
                Text = "",
                ContentViewType = typeof(DebugOptionsView),
            });

        return items;
    }

    partial void OnSelectedNavItemChanged(NavItem value)
    {
        StrongReferenceMessenger.Default.Send(new AppNavMessage(value));
    }

    [RelayCommand]
    private static async Task OpenSessionPage()
    {
        await Navigator.GoToPage<SessionPage>(modal: true);
    }
}
