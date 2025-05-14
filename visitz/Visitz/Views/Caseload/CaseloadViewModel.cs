using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.Specialized;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.SegmentedButtons;
using Visitz.Views.User;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Caseload
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        private static readonly string SortOptionIndexPref = "SortOptionIndexPref";

        private static readonly SegmentedOptions SortKeyPlayer = new(
            nameof(LocalizedStrings.KeyPlayer),
            LocalizedStrings.KeyPlayer,
            MaterialIcons.Person.GetUnfilledMaterialIcon());

        private static readonly SegmentedOptions SortOpenDate = new(
            nameof(IBusinessObject.DisplayDate),
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
        public CaseloadLister lister;

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

        [ObservableProperty]
        public SegmentedOptions activatedSortOption;

        [ObservableProperty]
        public SegmentedOptions activatedFilterOption;

        [ObservableProperty]
        public IList<SegmentedOptions> sortOptions = [SortKeyPlayer, SortOpenDate,];

        [ObservableProperty]
        public IList<SegmentedOptions> filterOptions =
        [
            FilterChildProtection,
            FilterChildServices,
            FilterFamilyServices,
        ];

        [ObservableProperty]
        public bool showAvatarView;

        private readonly ObservableRealmQueryMap realmQueryMap = new();

        [ObservableProperty]
        public HashSet<(string EntityId, EntityType Type)> draftedNotes = [];

        [ObservableProperty]
        public HashSet<(string EntityId, EntityType Type)> draftedAssessments = [];

        [ObservableProperty]
        public HashSet<(string EntityId, EntityType Type)> draftedAttachments = [];

        [ObservableProperty]
        public HashSet<(string EntityId, EntityType Type)> draftedItems = [];

        [ObservableProperty]
        public HashSet<(string EntityId, EntityType Type)> draftedVisits = [];

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

            Lister = new CaseloadLister(Realm, list =>
            {
                list = ApplySorting(list);
                list = ApplySearchQuery(list);
                list = ApplySubtypeFilter(list);
                return list;
            });

            Lister.Records.CollectionChanged += Records_CollectionChanged;

            realmQueryMap.ItemsChanged += RealmQueryMap_DraftsChanged;

            var noteDraft = await VisitzRealms.GetNoteDraftsRealmAsync();
            realmQueryMap.Subscribe(noteDraft, noteDraft.All<NoteDraft>());

            var assessmentDraft = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
            realmQueryMap.Subscribe(assessmentDraft, assessmentDraft.All<AssessmentDraft>());

            var attachmentDraft = await VisitzRealms.GetAttachmentDraftsRealmAsync();
            realmQueryMap.Subscribe(attachmentDraft, attachmentDraft.All<AttachmentDraft>());

            var visitDraft = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
            realmQueryMap.Subscribe(visitDraft, visitDraft.All<PersonVisitDraft>());
        }

        private void Records_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ApplyCollectionViewPrompt();
        }

        private void Teardown()
        {
            DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            realmQueryMap?.Dispose();

            Lister?.Dispose();

            Realm?.Dispose();
            Realm = null;
        }

        protected override async Task InitAsync()
        {
            await base.InitAsync();

            await Setup();
        }

        bool disposed;
        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                Teardown();

                disposed = true;
            }
            base.Dispose(disposing);
        }

        private IEnumerable<IBusinessObject> ApplySorting(IEnumerable<IBusinessObject> query)
        {
            if (query == null || ActivatedSortOption == null)
                return query;

            if (ActivatedSortOption == SortOpenDate)
            {
                query = query.OrderBy(IBusinessObjectExtensions.DisplayDateTransform);
            }
            else if (ActivatedSortOption == SortKeyPlayer)
            {
                var sort = new Func<IBusinessObject, string>(item => item.DisplayName);
                query = query.OrderBy(sort);
            }

            return query;
        }

        private IEnumerable<IBusinessObject> ApplySearchQuery(IEnumerable<IBusinessObject> query)
        {
            if (query == null || string.IsNullOrWhiteSpace(SearchQuery))
                return query;

            string trimmedSearch = SearchQuery.Trim();

            return query.Where(item =>
            {
                return item.FileNumber.Contains(trimmedSearch, StringComparison.InvariantCultureIgnoreCase)
                    || item.DisplayName.Contains(trimmedSearch, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        private IEnumerable<IBusinessObject> ApplySubtypeFilter(IEnumerable<IBusinessObject> query)
        {
            if (query == null || ActivatedFilterOption == null)
                return query;

            EntitySubtype subtype;

            if (ActivatedFilterOption.Id == nameof(EntitySubtype.ChildProtection))
                subtype = EntitySubtype.ChildProtection;
            else if (ActivatedFilterOption.Id == nameof(EntitySubtype.ChildServices))
                subtype = EntitySubtype.ChildServices;
            else if (ActivatedFilterOption.Id == nameof(EntitySubtype.FamilyServices))
                subtype = EntitySubtype.FamilyServices;
            else
                return query;

            return query.Where(item => item.EntitySubtype == subtype);
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
        public static async Task OpenSessionPage()
        {
            await Navigator.GoToPage<SessionPage>(modal: true);
        }

        public void SearchCaseload()
        {
            Lister.ApplyWithFilter();
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;

            if (message.FinishedSuccess)
                ShowEmptyCaseloadMessage = !Lister.Records.Any();
        }

        partial void OnActivatedSortOptionChanged(SegmentedOptions value)
        {
            Preferences.Default.Set(SortOptionIndexPref, SortOptions.IndexOf(value));
            Lister.ApplyWithFilter();
        }

        partial void OnActivatedFilterOptionChanged(SegmentedOptions value)
        {
            Lister.ApplyWithFilter();
            IsFilterActivated = value != null;
        }

        private void Current_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            ShowAvatarView = e.DisplayInfo.Orientation == DisplayOrientation.Portrait;
        }

        private void RealmQueryMap_DraftsChanged(
            object sender,
            (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e)
        {
            HashSet<(string EntityId, EntityType Type)> drafted = [];

            foreach (var item in e.Items.Cast<IDraftItem>())
                drafted.Add((item.RelatedEntityId, item.RelatedEntityType));

            if (e.Type == typeof(NoteDraft))
                DraftedNotes = drafted;
            else if (e.Type == typeof(AssessmentDraft))
                DraftedAssessments = drafted;
            else if (e.Type == typeof(AttachmentDraft))
                DraftedAttachments = drafted;
            else if (e.Type == typeof(PersonVisitDraft))
                DraftedVisits = drafted;
        }

        partial void OnDraftedNotesChanged(HashSet<(string EntityId, EntityType Type)> value)
        {
            var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
            newSet.UnionWith(DraftedAssessments);
            newSet.UnionWith(DraftedAttachments);
            newSet.UnionWith(DraftedVisits);
            DraftedItems = newSet;
        }

        partial void OnDraftedAssessmentsChanged(HashSet<(string EntityId, EntityType Type)> value)
        {
            var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
            newSet.UnionWith(DraftedNotes);
            newSet.UnionWith(DraftedAttachments);
            newSet.UnionWith(DraftedVisits);
            DraftedItems = newSet;
        }

        partial void OnDraftedAttachmentsChanged(HashSet<(string EntityId, EntityType Type)> value)
        {
            var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
            newSet.UnionWith(DraftedNotes);
            newSet.UnionWith(DraftedAssessments);
            newSet.UnionWith(DraftedVisits);
            DraftedItems = newSet;
        }

        partial void OnDraftedVisitsChanged(HashSet<(string EntityId, EntityType Type)> value)
        {
            var newSet = new HashSet<(string EntityId, EntityType Type)>(value);
            newSet.UnionWith(DraftedAssessments);
            newSet.UnionWith(DraftedAttachments);
            newSet.UnionWith(DraftedNotes);
            DraftedItems = newSet;
        }
    }
}
