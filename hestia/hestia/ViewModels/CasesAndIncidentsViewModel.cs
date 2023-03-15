using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using hestia.Models.DTOs;
using hestia.Models.Payloads;


using hestia.Services.Networking;


namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public class CasesAndIncidentsViewModel : BaseViewModel
    {
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
                HttpResponseMessage response = await httpClient.PostAsync("", nvcContent);
                string responseContent = await response.Content.ReadAsStringAsync();
                CaseIncidentListDTO dto = JsonSerializer.Deserialize<CaseIncidentListDTO>(responseContent, options: null);
                string caseIncidentNumber = dto.listCaseIncident.payLoad.listCaseIncidents.FirstOrDefault<ListCaseIncident2>().caseIncidentNumber;
                Console.WriteLine($"caseIncidentNumber is {caseIncidentNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"exception is {ex.ToString()}");
            }
        }
    }
}

