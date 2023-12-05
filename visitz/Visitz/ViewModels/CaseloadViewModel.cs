using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Messaging;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Storage;
using Visitz.Views.SegmentedButtons;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        private static readonly string SortOptionIndexPref = "SortOptionIndexPref";

        [ObservableProperty]
        public IEnumerable<CaseloadItem> caseload;

        [ObservableProperty]
        public CaseloadSort selectedSortOrder;

        [ObservableProperty]
        public bool isRefreshing;

        [ObservableProperty]
        public string searchQuery;

        [ObservableProperty]
        public bool showEmptyCaseloadMessage;

        [ObservableProperty]
        public string collectionViewPrompt;

        [ObservableProperty]
        public string subtypeFilter;

        private Realm Realm { get; set; }

        private IQueryable<CaseloadItem> CaseloadQuery { get; set; }

        private IDisposable CaseloadQueryToken { get; set; }

        [ObservableProperty]
        public SegmentedOptions activatedSortOption;

        [ObservableProperty]
        public SegmentedOptions activatedFilterOption;

        [ObservableProperty]
        public IList<SegmentedOptions> sortOptions = new List<SegmentedOptions>()
        {
            new()
            {
                Id = nameof(CaseloadItem.TryGetKeyPlayer),
                Text = LocalizedStrings.KeyPlayer,
                ImageSource = MaterialIcons.Person.GetUnfilledMaterialIcon(),
            },
            new()
            {
                Id = nameof(CaseloadItem.DisplayDate),
                Text = LocalizedStrings.OpenDate,
                ImageSource = MaterialIcons.Calendar_month.GetUnfilledMaterialIcon(),
            },
        };

        [ObservableProperty]
        public IList<SegmentedOptions> filterOptions = new List<SegmentedOptions>()
        {
            new()
            {
                Id = nameof(IcmEntitySubtype.ChildProtection),
                Text = LocalizedStrings.Subtype_ChildProtectionIncidentInitials,
                ImageSource = MaterialIcons.Warning.GetUnfilledMaterialIcon(),
            },
            new()
            {
                Id = nameof(IcmEntitySubtype.ChildServices),
                Text = LocalizedStrings.Subtype_ChildServicesInitials,
                ImageSource = MaterialIcons.Folder.GetUnfilledMaterialIcon(),
            },
            new()
            {
                Id = nameof(IcmEntitySubtype.FamilyServices),
                Text = LocalizedStrings.Subtype_FamilyServicesInitials,
                ImageSource = MaterialIcons.Folder.GetUnfilledMaterialIcon(),
            },
        };

        private async Task Setup()
        {
            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

            Realm = await VisitzRealm.GetIcmDataAsync();

            int sortPrefIndex = Preferences.Default.Get(SortOptionIndexPref, 0);
            ActivatedSortOption = SortOptions.ElementAt(sortPrefIndex);

            CaseloadQuery = Realm.All<CaseloadItem>();
            CaseloadQueryToken = CaseloadQuery.SubscribeForNotifications(Caseload_Changed);

            ShowEmptyCaseloadMessage = false;
            CollectionViewPrompt = LocalizedStrings.PullToRefreshCaseload;
        }

        private void Teardown()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            CaseloadQueryToken?.Dispose();
            CaseloadQueryToken = null;

            Realm?.Dispose();
            Realm = null;
        }

        public override async void PageCreated()
        {
            base.PageCreated();

            await Setup();

            ApplyCaseloadQuery();
        }

        public override void PageDestroyed()
        {
            Teardown();

            base.PageDestroyed();
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
            if (query == null || SelectedSortOrder == null)
                return;

            if (SelectedSortOrder.Id == CaseloadSort.DisplayDate)
            {
                query = SelectedSortOrder.Ascending
                    ? query.OrderBy(CaseloadItem.DisplayDateTransform)
                    : query.OrderByDescending(CaseloadItem.DisplayDateTransform);
            }
            else if (SelectedSortOrder.Id == CaseloadSort.DisplayName)
            {
                var sort = new Func<CaseloadItem, string>(item => item.DisplayName);

                query = SelectedSortOrder.Ascending
                    ? query.OrderBy(sort)
                    : query.OrderByDescending(sort);
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
            if (query == null || string.IsNullOrWhiteSpace(SubtypeFilter))
                return;

            query = query.Where(item => item.CaseIncidentType.Equals(SubtypeFilter));
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
            await SessionPage.OpenAsync(VisitzPage, true);
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

        partial void OnSubtypeFilterChanged(string value)
        {
            ApplyCaseloadQuery();
        }

        partial void OnActivatedSortOptionChanged(SegmentedOptions value)
        {
            Preferences.Default.Set(SortOptionIndexPref, SortOptions.IndexOf(value));
        }

        partial void OnActivatedFilterOptionChanged(SegmentedOptions value)
        {

        }
    }
}
