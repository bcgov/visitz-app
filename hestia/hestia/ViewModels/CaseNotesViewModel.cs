using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using hestia.Models.BOs;

namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class CaseNotesViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public Models.BOs.ListCaseIncident2 caseIncident;

        public ObservableCollection<CaseNoteBO> CaseNotes { get; set; } = new();

        private HttpClient httpClient;

        public CaseNotesViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async void FetchCaseNotes()
        {
            CaseIncidentNotesPayload payload = new();
            payload.requestGetNotes.payLoad.entityNumber = CaseIncident.caseIncidentNumber;
            payload.requestGetNotes.payLoad.entityType = CaseIncident.entityType;

            string json = JsonSerializer.Serialize<CaseIncidentNotesPayload>(payload, options: null);
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("docRequest", json));
            var nvcContent = new FormUrlEncodedContent(nvc);
            nvcContent.Headers.Clear();
            nvcContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            try
            {
                httpClient.BaseAddress = new Uri("https://hestia-dev.api.gov.bc.ca/v1/678");
                HttpResponseMessage response = await httpClient.PostAsync("", nvcContent);
                string responseContent = await response.Content.ReadAsStringAsync();
                CaseIncidentNotesDTO dto = JsonSerializer.Deserialize<CaseIncidentNotesDTO>(responseContent, options: null);
                CaseNotes.Clear();
                dto?.responseGetNotes?.payLoad?.notes?.ForEach(item =>
                {
                    CaseNotes.Add(CaseNoteBO.ToBO(item));
                });
            }
            catch (Exception ex)
            {

            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as Models.BOs.ListCaseIncident2;
        }
    }
}

