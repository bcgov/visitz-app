using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
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

        private Realm Realm { get; set; }

        private IQueryable<CaseloadItem> CaseloadQuery { get; set; }

        private IDisposable CaseloadQueryToken { get; set; }

        public override async void PageCreated()
        {
            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

            Realm = await VisitzRealm.GetIcmDataAsync();

            CaseloadQuery = Realm.All<CaseloadItem>();
            CaseloadQueryToken = CaseloadQuery.SubscribeForNotifications(Caseload_Changed);

            ApplyCaseloadQuery();
            ApplySubtypesQuery();
        }

        public override void PageDestroyed()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            CaseloadQueryToken.Dispose();
            CaseloadQueryToken = null;

            Realm.Dispose();
            Realm = null;
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
            if (SelectedSubtype == null || SelectedSubtype == FilterNoneOption)
                return;
            
            query = query.Where(item => item.CaseIncidentType == SelectedSubtype);
        }

        private void ApplySorting(ref IEnumerable<CaseloadItem> query)
        {
            if (SelectedSortOrder == null)
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

        [RelayCommand]
        public void RefreshCaseload()
        {
            WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage());
        }

        [RelayCommand]
        public async void GoToNotes(CaseloadItem caseloadItem)
        {
            await NotesPage.Open(VisitzPage, caseloadItem.CaseIncidentNumber);
        }

        public async Task OpenDebugOptionsPage()
        {
            if (DebugOptions.Enabled)
                await NavigateTo<DebugOptionsPage>();
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;
        }
    }
}

