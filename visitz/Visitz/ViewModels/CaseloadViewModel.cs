using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Authentication.Keycloak;
using Visitz.Extensions;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        private static readonly string FilterNoneOption = LocalizedStrings.All;

        [ObservableProperty]
        public IEnumerable<CaseloadItem> caseload;

        [ObservableProperty]
        public IEnumerable<string> subtypes;

        [ObservableProperty]
        public CaseloadSort selectedSortOrder;

        [ObservableProperty]
        public string selectedSubtype;

        [ObservableProperty]
        public bool isRefreshing;

        [ObservableProperty]
        public string sessionDisplayName;

        [ObservableProperty]
        public string searchQuery;

        [ObservableProperty]
        public bool showEmptyCaseloadMessage;

        [ObservableProperty]
        public string collectionViewPrompt;

        private Realm Realm { get; set; }

        private IQueryable<CaseloadItem> CaseloadQuery { get; set; }

        private IDisposable CaseloadQueryToken { get; set; }

        public override async void PageCreated()
        {
            base.PageCreated();

            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

            Realm = await VisitzRealm.GetIcmDataAsync();

            CaseloadQuery = Realm.All<CaseloadItem>();
            CaseloadQueryToken = CaseloadQuery.SubscribeForNotifications(Caseload_Changed);

            VisitzSession.SessionChanged += VisitzSession_SessionChanged;

            ShowEmptyCaseloadMessage = false;
            CollectionViewPrompt = LocalizedStrings.PullToRefreshCaseload;

            ApplyCaseloadQuery();
            ApplySubtypesQuery();
        }

        public override async void PageStarted()
        {
            base.PageStarted();

            SessionDisplayName = await SessionViewModel.GetDisplayNamePrompt();
        }

        public override void PageDestroyed()
        {
            VisitzSession.SessionChanged += VisitzSession_SessionChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            CaseloadQueryToken.Dispose();
            CaseloadQueryToken = null;

            Realm.Dispose();
            Realm = null;

            base.PageDestroyed();
        }

        private void Caseload_Changed(IRealmCollection<CaseloadItem> sender, ChangeSet changes)
        {
            if (changes == null)
                return;

            ApplyCaseloadQuery();
            ApplySubtypesQuery();
        }

        public void ApplyCaseloadQuery()
        {
            var query = CaseloadQuery.AsEnumerable();

            ApplySubtypeFiltering(ref query);
            ApplySorting(ref query);
            ApplySearchQuery(ref query);

            Caseload = query;
        }

        private void ApplySubtypesQuery()
        {
            var query = CaseloadQuery.AsEnumerable()
                .Select(item => item.CaseIncidentType)
                .Distinct()
                .Order()
                .ToList();

            query.Insert(0, FilterNoneOption);

            Subtypes = query;
        }

        private void ApplySubtypeFiltering(ref IEnumerable<CaseloadItem> query)
        {
            if (query == null || SelectedSubtype == null || SelectedSubtype == FilterNoneOption)
                return;
            
            query = query.Where(item => item.CaseIncidentType == SelectedSubtype);
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

            query = query.Where(item =>
            {
                return item.CaseIncidentNumber.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase)
                    || item.DisplayName.Contains(SearchQuery, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        partial void OnCaseloadChanged(IEnumerable<CaseloadItem> value)
        {
            ApplyCollectionViewPrompt();
        }

        private void ApplyCollectionViewPrompt()
        {
            if (IsSubtypeSelected() && !string.IsNullOrWhiteSpace(SearchQuery))
            {
                CollectionViewPrompt = LocalizedStrings.NoResultsForSearchAndFilter
                    .Format(SelectedSubtype, SearchQuery);
            }
            else if (IsSubtypeSelected())
            {
                CollectionViewPrompt = LocalizedStrings.NoResultsForSearch.Format(SelectedSubtype);
            }
            else if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                CollectionViewPrompt = LocalizedStrings.NoResultsForSearch.Format(SearchQuery);
            }
            else
            {
                CollectionViewPrompt = LocalizedStrings.PullToRefreshCaseload;
            }
        }

        private bool IsSubtypeSelected()
        {
            return SelectedSubtype != null && SelectedSubtype != FilterNoneOption;
        }

        [RelayCommand]
        public void RefreshCaseload()
        {
            WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage());
            ShowEmptyCaseloadMessage = false;
        }

        [RelayCommand]
        public async void GoToNotes(CaseloadItem caseloadItem)
        {
            await NotesPage.Open(VisitzPage, caseloadItem.CaseIncidentNumber);
        }

        [RelayCommand]
        public async void OpenDebugOptionsPage()
        {
            if (DebugOptions.Enabled)
                await NavigateTo<DebugOptionsPage>();
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

        private async void VisitzSession_SessionChanged(object sender, EventArgs e)
        {
            SessionDisplayName = await SessionViewModel.GetDisplayNamePrompt(sender as VisitzSessionInfo);
        }
    }
}
