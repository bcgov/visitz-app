using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using hestiapi;
using hestia.Models.BOs;

namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : BaseViewModel
    {
        public ObservableCollection<CaseloadItem> Caseload { get; set; } = new();

        [ObservableProperty]
        public CaseloadItem selectedCaseIncident;

        private HttpClient httpClient;

        public CaseloadViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async void FetchCasesAndIncidents()
        {
            try
            {
                // TODO: Base URL should be read from an ApiSettings implementation
                var api = new HestiApi(httpClient, "https://hestia-dev.api.gov.bc.ca");

                // TODO: Worker ID should be collected from current JWT Access Token field "idir_username"
                var caseloadContent = await api.GetCaseloadAsync("CGWRK68");

                Caseload.Clear();

                foreach (var item in caseloadContent)
                    Caseload.Add(new CaseloadItem(item));
            }
            catch (HestiaApiException ex)
            {
                // TODO: Make actual error UI/UX to show this error
                Console.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        void GoToNotes(CaseloadItem caseloadItem)
        {
            SelectedCaseIncident = caseloadItem;
        }
    }
}

