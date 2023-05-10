using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using hestia.Models.BOs;
using hestiapi;

namespace hestia.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class CaseNotesViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public ListCaseIncident2 caseIncident;

        public ObservableCollection<NoteItem> CaseNotes { get; set; } = new();

        private HttpClient httpClient;

        public CaseNotesViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async void FetchCaseNotes()
        {
            try
            {
                // TODO: Base URL should be read from an ApiSettings implementation
                var api = new HestiApi(httpClient, "https://hestia-dev.api.gov.bc.ca");

                var notesList = await api.GetNotesAsync(CaseIncident.caseIncidentNumber, CaseIncident.entityType);

                CaseNotes.Clear();

                foreach (var note in notesList)
                    CaseNotes.Add(new NoteItem(note));
            }
            catch (Exception ex)
            {

            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as ListCaseIncident2;
        }
    }
}

