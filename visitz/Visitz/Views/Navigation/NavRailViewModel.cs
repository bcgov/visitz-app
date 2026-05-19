using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Extensions;
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

#nullable enable

public partial class NavRailViewModel : VisitzViewModel
{
#if IOS
    static readonly double IosIconSize = 34;
#else
    static readonly double DefaultIconSize = 21;
#endif

    [ObservableProperty]
    public partial ObservableCollection<NavItem> NavigationItems { get; set; } = [];

    [ObservableProperty]
    public partial NavItem? SelectedNavItem { get; set; }

    private IDisposable? _personVisitToken;
    private Realm? _icmDataRealm;

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
    public partial NavItem TodoNavItem { get; set; } =
        new()
        {
            Text = LocalizedStrings.Todo,
            ContentViewType = typeof(TodoContainerView),
            Color = Colors.White,
            IconSize = IconSize,
            SelectedImageSource = MaterialIcons.Checklist.GetFilledMaterialIcon(Colors.White),
            UnselectedImageSource = MaterialIcons.Checklist.GetUnfilledMaterialIcon(Colors.White),
        };

    [ObservableProperty]
    public partial NavItem CaseloadNavItem { get; set; } =
        new()
        {
            Text = LocalizedStrings.Caseload,
            ContentViewType = typeof(CaseloadContainerView),
            Color = Colors.White,
            IconSize = IconSize,
            SelectedImageSource = MaterialIcons.Folder_open.GetFilledMaterialIcon(Colors.White),
            UnselectedImageSource = MaterialIcons.Folder_open.GetUnfilledMaterialIcon(Colors.White),
        };

    [ObservableProperty]
    public partial NavItem DraftsNavItem { get; set; } =
        new()
        {
            Text = LocalizedStrings.Drafts,
            ContentViewType = typeof(DraftsContainerView),
            Color = Colors.White,
            IconSize = IconSize,
            SelectedImageSource = MaterialIcons.Draft.GetFilledMaterialIcon(Colors.White),
            UnselectedImageSource = MaterialIcons.Draft.GetUnfilledMaterialIcon(Colors.White),
        };

    readonly ObservableRealmCount realmCount = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        BuildNavCollection();

        await SubscribeToAllDraftCounts();

        StrongReferenceMessenger.Default.Register<AppNavMessage>(this, ReceiveAppNavMessage);
        StrongReferenceMessenger.Default.Register<TodoBadgeCountMessage>(this, ReceiveTodoBadgeCountMessage);

        await SubscribeToAllTodoCounts();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmCount.Dispose();
            _personVisitToken?.Dispose();
            _icmDataRealm?.Dispose();
            StrongReferenceMessenger.Default.Unregister<TodoBadgeCountMessage>(this);
            realmCount.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void BuildNavCollection()
    {
        NavigationItems.Clear();

        NavigationItems.Add(TodoNavItem);
        NavigationItems.Add(CaseloadNavItem);
        NavigationItems.Add(DraftsNavItem);
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

        var query = _icmDataRealm.All<PersonVisit>();
        var collection = query.AsRealmCollection();

        _personVisitToken = collection.SubscribeForNotifications(
            (sender, changes) =>
            {
                int updatedCount = PersonVisit
                    .GetLatestVisitsPerParentRecord(_icmDataRealm)
                    .Count(PersonVisit.IsUpcomingVisit);
                StrongReferenceMessenger.Default.Send(new TodoBadgeCountMessage(updatedCount));

                if (changes == null)
                    FirstLoadNavigate(updatedCount > 0 ? TodoNavItem : CaseloadNavItem);
            }
        );
    }

    private void FirstLoadNavigate(NavItem navItem)
    {
        SelectedNavItem = navItem;
    }

    partial void OnSelectedNavItemChanged(NavItem? value)
    {
        if (value == null)
            return;

        StrongReferenceMessenger.Default.Send(new AppNavMessage(value));
    }

    [RelayCommand]
    private static async Task OpenSessionPage()
    {
        var userView = ServiceProvider.GetService<UserView>();
        await Navigator.Navigation.PushModalAsync(userView);
    }

    private void RealmCount_CountChanged(object? sender, (Type Kind, int Count) e)
    {
        DraftsNavItem.BadgeCount = (sender as ObservableRealmCount)?.Total ?? 0;
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

    private NavItem? GetNavItemByType(Type? contentViewType)
    {
        return NavigationItems.FirstOrDefault(item => item.ContentViewType == contentViewType);
    }

    [RelayCommand]
    private static async Task OpenDebugOptions()
    {
        if (DebugOptions.Enabled)
            await Navigator.GoToPage<DebugOptionsPage>();
    }
}
