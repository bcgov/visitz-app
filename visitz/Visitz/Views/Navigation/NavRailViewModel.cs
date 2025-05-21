using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.ObjectModel;
using Visitz.FontIcons;
using Visitz.Messaging;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.Caseload;
using Visitz.Views.Debugging;
using Visitz.Views.Drafts;
using Visitz.Views.Todo;
using Visitz.Views.User;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Navigation;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Navigation;

public partial class NavRailViewModel : VisitzViewModel
{
#if IOS
    static readonly double IosIconSize = 34;
#else
    static readonly double DefaultIconSize = 24;
#endif

    [ObservableProperty]
    public ObservableCollection<object> navigationItems = [];

    [ObservableProperty]
    public NavItem selectedNavItem;

    private IDisposable _personVisitToken;
    private Realm _icmDataRealm;

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
    public NavItem todoNavItem = new()
    {
        Text = LocalizedStrings.Todo,
        ContentViewType = typeof(TodoContainerView),
        Color = Colors.White,
        IconSize = IconSize,
        SelectedImageSource = MaterialIcons.Checklist.GetFilledMaterialIcon(Colors.White),
        UnselectedImageSource = MaterialIcons.Checklist.GetUnfilledMaterialIcon(Colors.White),
    };

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
        StrongReferenceMessenger.Default.Register<TodoBadgeCountMessage>(this, ReceiveTodoBadgeCountMessage);
        await SubscribeToAllTodoCounts();
    }

    public override void Destroy()
    {
        base.Destroy();

        _personVisitToken?.Dispose();
        _icmDataRealm?.Dispose();
        StrongReferenceMessenger.Default.Unregister<TodoBadgeCountMessage>(this);
        realmCount.Dispose();
    }

    private void BuildNavCollection()
    {
        NavigationItems.Clear();

        NavigationItems.Add(TodoNavItem);
        NavigationItems.Add(CaseloadNavItem);
        NavigationItems.Add(DraftsNavItem);

        if (DebugOptions.Enabled)
            NavigationItems.Add(DebugNavItem);
    }

    private async Task SubscribeToAllDraftCounts()
    {
        realmCount.CountChanged += RealmCount_CountChanged;

        realmCount.Subscribe<AttachmentDraft>(await VisitzRealms.GetAttachmentDraftsRealmAsync());
        realmCount.Subscribe<NoteDraft>(await VisitzRealms.GetNoteDraftsRealmAsync());
        realmCount.Subscribe<AssessmentDraft>(await VisitzRealms.GetSafetyAssessmentDraftRealmAsync());
        realmCount.Subscribe<PersonVisitDraft>(await VisitzRealms.GetPersonVisitDraftsRealmAsync());
    }

    private async Task SubscribeToAllTodoCounts()
    {
        _icmDataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        var query = PersonVisit.GetAllByType(_icmDataRealm);
        var collection = query.AsRealmCollection();

        _personVisitToken = collection.SubscribeForNotifications((sender, changes) =>
        {
            int updatedCount = PersonVisit.GetUpcomingVisits(_icmDataRealm).Count();
            StrongReferenceMessenger.Default.Send(new TodoBadgeCountMessage(updatedCount));
        });
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


    private void ReceiveTodoBadgeCountMessage(object recipient, TodoBadgeCountMessage message)
    {
        TodoNavItem.BadgeCount = message.Value;
    }

    private NavItem GetNavItemByType(Type contentViewType)
    {
        return (NavItem)NavigationItems.FirstOrDefault(item => (item as NavItem).ContentViewType == contentViewType);
    }
}
