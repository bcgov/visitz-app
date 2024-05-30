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
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.SafetyAssess;

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
            nameof(EntitySubtype.ChildProtection), 
            LocalizedStrings.Subtype_ChildProtectionIncidentInitials, 
            MaterialIcons.Description.GetUnfilledMaterialIcon());
        
        private static readonly SegmentedOptions FilterChildServices = new(
            nameof(EntitySubtype.ChildServices), 
            LocalizedStrings.Subtype_ChildServicesInitials, 
            MaterialIcons.Folder.GetUnfilledMaterialIcon());
        
        private static readonly SegmentedOptions FilterFamilyServices = new(
            nameof(EntitySubtype.FamilyServices), 
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

		private readonly ObservableRealmQueryMap realmQueryMap = new();

		[ObservableProperty]
		public HashSet<(string EntityId, EntityType Type)> draftedNotes = [];

		[ObservableProperty]
		public HashSet<(string EntityId, EntityType Type)> draftedAssessments = [];

		[ObservableProperty]
		public HashSet<(string EntityId, EntityType Type)> draftedItems = [];

        private async Task Setup()
        {
            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

			await SetupRealm();

            int sortPrefIndex = Preferences.Default.Get(SortOptionIndexPref, 0);
            ActivatedSortOption = SortOptions.ElementAt(sortPrefIndex);

            ShowEmptyCaseloadMessage = false;
            CollectionViewPrompt = LocalizedStrings.PullToRefreshCaseload;

            DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
            ShowAvatarView = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait;
        }

		private async Task SetupRealm()
		{
			Realm = await VisitzRealms.GetIcmDataRealmAsync();

			CaseloadQuery = Realm.All<CaseloadItem>();
			CaseloadQueryToken = CaseloadQuery.SubscribeForNotifications(Caseload_Changed);

			realmQueryMap.ItemsChanged += RealmQueryMap_DraftsChanged;

			var noteDraft = await VisitzRealms.GetNoteDraftsRealmAsync();
			realmQueryMap.Subscribe(noteDraft, noteDraft.All<NoteDraft>());

			var assessmentDraft = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
			realmQueryMap.Subscribe(assessmentDraft, assessmentDraft.All<AssessmentDraft>());
		}

		private void Teardown()
        {
            DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);

			realmQueryMap.Dispose();

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

            if (ActivatedFilterOption.Id == nameof(EntitySubtype.ChildProtection))
                subtype = EntitySubtype.ChildProtection.GetDisplayString();
            else if (ActivatedFilterOption.Id == nameof(EntitySubtype.ChildServices))
                subtype = EntitySubtype.ChildServices.GetDisplayString();
            else if (ActivatedFilterOption.Id == nameof(EntitySubtype.FamilyServices))
                subtype = EntitySubtype.FamilyServices.GetDisplayString();
            else
                return;

			subtype = subtype.ToLowerInvariant();
            query = query.Where(item => item.CaseIncidentType.ToLowerInvariant().Equals(subtype));
        }

        partial void OnCaseloadChanged(IEnumerable<CaseloadItem> value)
        {
            ApplyCollectionViewPrompt();
        }

        private void ApplyCollectionViewPrompt()
        {
            CollectionViewPrompt = !string.IsNullOrWhiteSpace(SearchQuery)
                ? LocalizedStrings.NoResultsForSearch.Format(SearchQuery)
                : LocalizedStrings.PullToRefreshCaseload;
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
        public async Task OpenDebugOptionsPage()
        {
            await DebugOptionsPage.TryOpen();
        }

        [RelayCommand]
        public async Task OpenSessionPage()
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

		private void RealmQueryMap_DraftsChanged(object sender, (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
		{
			HashSet<(string EntityId, EntityType Type)> drafted = [];

			foreach (var item in e.Items.Cast<IDraftItem>())
				drafted.Add((item.RelatedEntityId, item.RelatedEntityType));

			if (e.Type == typeof(NoteDraft))
				DraftedNotes = drafted;
			else if (e.Type == typeof(AssessmentDraft))
				DraftedAssessments = drafted;
		}

		partial void OnDraftedNotesChanged(HashSet<(string EntityId, EntityType Type)> value)
		{
			var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
			newSet.UnionWith(DraftedAssessments);
			DraftedItems = newSet;
		}

		partial void OnDraftedAssessmentsChanged(HashSet<(string EntityId, EntityType Type)> value)
		{
			var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
			newSet.UnionWith(DraftedNotes);
			DraftedItems = newSet;
		}
	}
}
