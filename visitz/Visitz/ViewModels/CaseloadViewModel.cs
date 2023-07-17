using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System.Collections.Generic;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
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

        public override async void PageCreated()
        {
            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

            Realm = await IcmDataRealm.GetAsync();

            ApplyQuery();

            RefreshSubtypes();
        }

        private void RefreshSubtypes()
        {
            Subtypes = GetCaseloadSubtypes();
        }

        private IList<string> GetCaseloadSubtypes()
        {
            return Realm.All<CaseloadItem>()
                .AsEnumerable()
                .Select(item => item.CaseIncidentType)
                .Distinct()
                .Order()
                .ToList();
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

        public void ApplyQuery()
        {
            var query = Realm
                .All<CaseloadItem>()
                .AsEnumerable();

            ApplySubtypeFiltering(ref query);
            ApplySorting(ref query);

            Caseload = query;
        }

        private void ApplySubtypeFiltering(ref IEnumerable<CaseloadItem> query)
        {
            if (SelectedSubtype == null)
                return;
            
            query = query.Where(item => item.CaseIncidentType == SelectedSubtype);
        }

        private void ApplySorting(ref IEnumerable<CaseloadItem> query)
        {
            if (SelectedSortOrder == null)
                return;

            if (SelectedSortOrder.Id == CaseloadSort.DisplayDate)
            {
                var sort = new Func<CaseloadItem, DateTime>(item => {

                    return item.DisplayDate?.Length > 0 
                        ? DateTime.Parse(item.DisplayDate)
                        : DateTime.MinValue;
                });

                query = SelectedSortOrder.Ascending
                    ? query.OrderBy(sort)
                    : query.OrderByDescending(sort);
            }
            else if (SelectedSortOrder.Id == CaseloadSort.DisplayName)
            {
                var sort = new Func<CaseloadItem, string>(item => item.DisplayName);

                query = SelectedSortOrder.Ascending
                    ? query.OrderBy(sort)
                    : query.OrderByDescending(sort);
            }
        }

        public async Task OpenDebugOptionsPage()
        {
            if (DebugOptions.Enabled)
                await NavigateTo<DebugOptionsPage>();
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;

            if (message.FinishedSuccess)
            {
                RefreshSubtypes();
                ApplyQuery();
            }
        }
    }
}

