using System.Text.Json;
using hestia.Models.DTOs;
using hestia.Models.Payloads;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CasesAndIncidentsViewModel : BaseViewModel
    {
        public ObservableCollection<Models.BOs.ListCaseIncident2> BoCasesAndInsidents { get; set; } = new();
        [ObservableProperty]
        public Models.BOs.ListCaseIncident2 selectedCaseIncident;

        private HttpClient httpClient;
        public CasesAndIncidentsViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async void FetchCasesAndIncidents()
        {
            CaseIncidentListPayload payload = new();
            payload.getListCaseIncident.payLoad.workerIds.Add(new(workerId: "CGWRK68"));
            string json = JsonSerializer.Serialize<CaseIncidentListPayload>(payload, options: null);
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("docRequest", json));
            var nvcContent = new FormUrlEncodedContent(nvc);
            nvcContent.Headers.Clear();
            nvcContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            try
            {
                httpClient.BaseAddress = new Uri("https://hestia-dev.api.gov.bc.ca/v1/620b");
                HttpResponseMessage response = await httpClient.PostAsync("", nvcContent);
                string responseContent = await response.Content.ReadAsStringAsync();
                CaseIncidentListDTO dto = JsonSerializer.Deserialize<CaseIncidentListDTO>(responseContent, options: null);
                string caseIncidentNumber = dto.listCaseIncident.payLoad.listCaseIncidents.FirstOrDefault<Models.DTOs.ListCaseIncident2>().caseIncidentNumber;
                BoCasesAndInsidents.Clear();
                var caseIncidentListBO = Models.BOs.CaseIncidentListBO.ToBO(dto);
                caseIncidentListBO?.listCaseIncident?.payLoad?.listCaseIncidents?.ForEach(item =>
                {
                    BoCasesAndInsidents.Add(item);
                });
            }
            catch (Exception ex)
            {

            }
        }

        [RelayCommand]
        void GoToNotes(Models.BOs.ListCaseIncident2 caseIncident)
        {
            SelectedCaseIncident = caseIncident;
        }
    }
}

