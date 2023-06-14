using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using VisitzApi;
using Visitz.Models;
using VisitzApi.ErrorHandling;
using Visitz.Views;
using Visitz.Storage;
using Visitz.Authentication.Keycloak;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel
    {
        [ObservableProperty]
        public IEnumerable<CaseloadItem> caseload;

        private Vpi Vpi { get; set; }

        [ObservableProperty]
        public bool isRefreshing;

        public override async void PageCreated()
        {
            Vpi = VisitzApp.VisitzServices.GetRequiredService<Vpi>();

            var realm = await IcmDataRealm.GetAsync();
            Caseload = realm.All<CaseloadItem>();
        }

        public async Task TryFetchCasesAndIncidents()
        {
            if (await VisitzSession.GetValidSessionAsync())
                await FetchCasesAndIncidents();
        }

        public async Task FetchCasesAndIncidents()
        {
            try
            {
                var info = await VisitzSessionInfo.GetAsync();
                var caseloadFromApi = await Vpi.GetCaseloadAsync(info.Idir);
                var caseloadContent = CaseloadItem.FromApiEntities(caseloadFromApi);

                using var realm = await IcmDataRealm.GetAsync();
                await realm.WriteAsync(() =>
                {
                    realm.Add(caseloadContent, update: true);
                });
            }
            catch (VisitzApiException ex)
            {
                // TODO: Implement proper error logging/handling (show to user? store errors somewhere?)
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Implement proper error logging/handling (show to user? store errors somewhere?)
                Console.WriteLine(ex.StackTrace);
            }
        }

        [RelayCommand]
        public async Task RefreshCaseload()
        {
            try
            {
                await TryFetchCasesAndIncidents();
            }
            finally
            {
                IsRefreshing = false;
            }
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
    }
}

