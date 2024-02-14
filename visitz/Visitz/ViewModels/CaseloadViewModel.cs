using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.FontIcons;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Storage;
using Visitz.Views.SegmentedButtons;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        private static readonly string SortOptionIndexPref = "SortOptionIndexPref";

        private static readonly SegmentedOptions SortKeyPlayer = new(
            nameof(CaseloadItem.TryGetKeyPlayer),
            LocalizedStrings.KeyPlayer,
            MaterialIcons.Person.GetUnfilledMaterialIcon());

        private static readonly SegmentedOptions SortOpenDate = new(
            nameof(CaseloadItem.DisplayDate),
            LocalizedStrings.OpenDate,
            MaterialIcons.Calendar_month.GetUnfilledMaterialIcon());

        private static readonly SegmentedOptions FilterChildProtection = new(
            nameof(IcmEntitySubtype.ChildProtection), 
            LocalizedStrings.Subtype_ChildProtectionIncidentInitials, 
            MaterialIcons.Description.GetUnfilledMaterialIcon());
        
        private static readonly SegmentedOptions FilterChildServices = new(
            nameof(IcmEntitySubtype.ChildServices), 
            LocalizedStrings.Subtype_ChildServicesInitials, 
            MaterialIcons.Folder.GetUnfilledMaterialIcon());
        
        private static readonly SegmentedOptions FilterFamilyServices = new(
            nameof(IcmEntitySubtype.FamilyServices), 
            LocalizedStrings.Subtype_FamilyServicesInitials, 
            MaterialIcons.Folder.GetUnfilledMaterialIcon());

        [ObservableProperty]
        public IEnumerable<CaseloadItem> caseload;

        [ObservableProperty]
        public bool isRefreshing;

        [ObservableProperty]
        public string searchQuery;

        [ObservableProperty]
        public bool showEmptyCaseloadMessage;

        [ObservableProperty]
        public string collectionViewPrompt;

        [ObservableProperty]
        public bool isFilterActivated;

        private Realm Realm { get; set; }

        private IQueryable<CaseloadItem> CaseloadQuery { get; set; }

        private IDisposable CaseloadQueryToken { get; set; }

        [ObservableProperty]
        public SegmentedOptions activatedSortOption;

        [ObservableProperty]
        public SegmentedOptions activatedFilterOption;

        [ObservableProperty]
        public IList<SegmentedOptions> sortOptions = new List<SegmentedOptions>() { SortKeyPlayer, SortOpenDate, };

        [ObservableProperty]
        public IList<SegmentedOptions> filterOptions = new List<SegmentedOptions>()
        {
            FilterChildProtection, FilterChildServices, FilterFamilyServices,
        };

        [ObservableProperty]
        public bool showAvatarView;

        private async Task Setup()
        {
            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

            Realm = await VisitzRealms.GetIcmDataRealmAsync();

            int sortPrefIndex = Preferences.Default.Get(SortOptionIndexPref, 0);
            ActivatedSortOption = SortOptions.ElementAt(sortPrefIndex);

            CaseloadQuery = Realm.All<CaseloadItem>();
            CaseloadQueryToken = CaseloadQuery.SubscribeForNotifications(Caseload_Changed);

            ShowEmptyCaseloadMessage = false;
            CollectionViewPrompt = LocalizedStrings.PullToRefreshCaseload;

            DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
            ShowAvatarView = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait;
        }

        private void Teardown()
        {
            DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            CaseloadQueryToken?.Dispose();
            CaseloadQueryToken = null;

            Realm?.Dispose();
            Realm = null;
        }

        public override async void Create()
        {
            base.Create();

            await Setup();

            ApplyCaseloadQuery();
        }

        public override void Destroy()
        {
            Teardown();

            base.Destroy();
        }

        private void Caseload_Changed(IRealmCollection<CaseloadItem> sender, ChangeSet changes)
        {
            if (changes == null)
                return;

            ApplyCaseloadQuery();
        }

        public void ApplyCaseloadQuery()
        {
            var query = CaseloadQuery.AsEnumerable();

            ApplySorting(ref query);
            ApplySearchQuery(ref query);
            ApplySubtypeFilter(ref query);

            Caseload = query;
        }

        private void ApplySorting(ref IEnumerable<CaseloadItem> query)
        {
            if (query == null || ActivatedSortOption == SegmentedOptions.Empty)
                return;

            if (ActivatedSortOption == SortOpenDate)
            {
                query = query.OrderBy(CaseloadItem.DisplayDateTransform);
            }
            else if (ActivatedSortOption == SortKeyPlayer)
            {
                var sort = new Func<CaseloadItem, string>(item => item.DisplayName);
                query = query.OrderBy(sort);
            }
        }

        private void ApplySearchQuery(ref IEnumerable<CaseloadItem> query)
        {
            if (query == null || string.IsNullOrWhiteSpace(SearchQuery))
                return;

            string trimmedSearch = SearchQuery.Trim();

            query = query.Where(item =>
            {
                return item.CaseIncidentNumber.Contains(trimmedSearch, StringComparison.InvariantCultureIgnoreCase)
                    || item.DisplayName.Contains(trimmedSearch, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        private void ApplySubtypeFilter(ref IEnumerable<CaseloadItem> query)
        {
            if (query == null || ActivatedFilterOption == SegmentedOptions.Empty)
                return;

            string subtype;

            if (ActivatedFilterOption.Id == nameof(IcmEntitySubtype.ChildProtection))
                subtype = IcmEntitySubtype.ChildProtection;
            else if (ActivatedFilterOption.Id == nameof(IcmEntitySubtype.ChildServices))
                subtype = IcmEntitySubtype.ChildServices;
            else if (ActivatedFilterOption.Id == nameof(IcmEntitySubtype.FamilyServices))
                subtype = IcmEntitySubtype.FamilyServices;
            else
                return;

            query = query.Where(item => item.CaseIncidentType.Equals(subtype));
        }

        partial void OnCaseloadChanged(IEnumerable<CaseloadItem> value)
        {
            ApplyCollectionViewPrompt();
        }

        private void ApplyCollectionViewPrompt()
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                CollectionViewPrompt = LocalizedStrings.NoResultsForSearch.Format(SearchQuery);
            }
            else
            {
                CollectionViewPrompt = LocalizedStrings.PullToRefreshCaseload;
            }
        }

        [RelayCommand]
        public void RefreshCaseload()
        {
            WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage());
            ShowEmptyCaseloadMessage = false;
        }

        [RelayCommand]
        public static void CaseloadItemSelected(CaseloadItem caseloadItem)
        {
            StrongReferenceMessenger.Default.Send(new CaseloadItemSelectedMessage(caseloadItem));
        }

        [RelayCommand]
        public async void OpenDebugOptionsPage()
        {
            await DebugOptionsPage.TryOpen();
        }

        [RelayCommand]
        public async void OpenSessionPage()
        {
            await Navigator.GoToPage<SessionPage>(modal: true);
        }

        public void SearchCaseload()
        {
            ApplyCaseloadQuery();
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;

            if (message.FinishedSuccess)
                ShowEmptyCaseloadMessage = !CaseloadQuery.Any();
        }

        partial void OnActivatedSortOptionChanged(SegmentedOptions value)
        {
            Preferences.Default.Set(SortOptionIndexPref, SortOptions.IndexOf(value));
            ApplyCaseloadQuery();
        }

        partial void OnActivatedFilterOptionChanged(SegmentedOptions value)
        {
            ApplyCaseloadQuery();
            IsFilterActivated = value != SegmentedOptions.Empty;
        }

        private void Current_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            ShowAvatarView = e.DisplayInfo.Orientation == DisplayOrientation.Portrait;
        }
    }
}
