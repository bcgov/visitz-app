using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Visitz.FontIcons;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;
using Visitz.Views.Drafts;
using Visitz.Views.User;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Navigation;

public partial class NavRailViewModel : VisitzViewModel
{
	static readonly double IosIconSize = 34;
	static readonly double DefaultIconSize = 24;

	[ObservableProperty]
    public ObservableCollection<object> navigationItems = [];

    [ObservableProperty]
    public NavItem selectedNavItem;

	public static double IconSize
	{
		get
		{
#if IOS
		return IosIconSize;
#else
		return DefaultIconSize;
#endif
		}
	}

	[ObservableProperty]
	public NavItem caseloadNavItem = new()
	{
		Text = LocalizedStrings.Caseload,
		ContentViewType = typeof(CaseloadContainerView),
		Color = Colors.White,
		IconSize = IconSize,
		SelectedImageSource = MaterialIcons.Folder_open.GetFilledMaterialIcon(Colors.White),
		UnselectedImageSource = MaterialIcons.Folder_open.GetUnfilledMaterialIcon(Colors.White),
	};

	[ObservableProperty]
	public NavItem draftsNavItem = new()
	{
		Text = LocalizedStrings.Drafts,
		ContentViewType = typeof(DraftsContainerView),
		Color = Colors.White,
		IconSize = IconSize,
		SelectedImageSource = MaterialIcons.Draft.GetFilledMaterialIcon(Colors.White),
		UnselectedImageSource = MaterialIcons.Draft.GetUnfilledMaterialIcon(Colors.White),
	};

	[ObservableProperty]
	public NavItem debugNavItem = new()
	{
		Text = "",
		ContentViewType = typeof(DebugOptionsView),
	};

	readonly ObservableRealmCount realmCount = new();

	public override async void Create()
    {
        base.Create();

        BuildNavCollection();
        SelectedNavItem = (NavItem)NavigationItems.First();

		await SubscribeToAllDraftCounts();

		StrongReferenceMessenger.Default.Register<AppNavMessage>(this, ReceiveAppNavMessage);
	}

	public override void Destroy()
	{
		base.Destroy();

		realmCount.Dispose();
	}

	private void BuildNavCollection()
    {
		NavigationItems.Clear();

		NavigationItems.Add(CaseloadNavItem);
		NavigationItems.Add(DraftsNavItem);

		if (DebugOptions.Enabled)
			NavigationItems.Add(DebugNavItem);
    }

	private async Task SubscribeToAllDraftCounts()
	{
		realmCount.CountChanged += RealmCount_CountChanged;

		realmCount.Subscribe<NoteDraft>(await VisitzRealms.GetNoteDraftsRealmAsync());
		realmCount.Subscribe<AssessmentDraft>(await VisitzRealms.GetSafetyAssessmentDraftRealmAsync());
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

	private void RealmCount_CountChanged(object sender, (Type Kind, int Count) e)
	{
		DraftsNavItem.BadgeCount = (sender as ObservableRealmCount).Total;
	}

	private void ReceiveAppNavMessage(object recipient, AppNavMessage message)
	{
		if (message.Value != null && SelectedNavItem != message.Value)
			SelectedNavItem = GetNavItemByType(message.Value.ContentViewType);
	}

	private NavItem GetNavItemByType(Type contentViewType)
	{
		return (NavItem)NavigationItems.FirstOrDefault(item => (item as NavItem).ContentViewType == contentViewType);
	}
}
