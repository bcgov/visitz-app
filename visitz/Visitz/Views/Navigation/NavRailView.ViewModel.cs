using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Authentication.Keycloak.Events;
using Visitz.FontIcons;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Services;
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

    [ObservableProperty]
    public string initials;

    public override async void PageCreated()
    {
        base.PageCreated();

        NavigationItems = BuildNavItems();
        SelectedNavItem = NavigationItems.First();

        VisitzSession.SessionChanged += VisitzSession_SessionChanged;
        await SetInitials();
    }

    public override void PageDestroyed()
    {
        VisitzSession.SessionChanged -= VisitzSession_SessionChanged;

        base.PageDestroyed();
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
            }
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

    private async Task SetInitials()
    {
        var info = await VisitzSessionInfo.GetAsync();

        Initials = info.UserInitials;
    }

    private async void VisitzSession_SessionChanged(object sender, SessionChangedEventArgs e)
    {
        await SetInitials();
    }

    [RelayCommand]
    private static async void OpenSessionPage()
    {
        await Navigator.GoToPage<SessionPage>(modal: true);
    }

    [RelayCommand]
    public static void RefreshCaseload()
    {
        WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage());
    }
}
