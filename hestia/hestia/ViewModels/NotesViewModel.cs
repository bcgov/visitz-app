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
	public partial class NotesViewModel : BaseViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public CaseloadItem caseIncident;

        public ObservableCollection<NoteItem> Notes { get; set; } = new();

        private HttpClient httpClient;

        public NotesViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async void FetchNotes()
        {
            try
            {
                // TODO: Base URL should be read from an ApiSettings implementation
                var api = new HestiApi(httpClient, "https://hestia-dev.api.gov.bc.ca");

                var notesList = await api.GetNotesAsync(CaseIncident.CaseIncidentNumber, CaseIncident.EntityType);

                Notes.Clear();

                foreach (var note in notesList)
                    Notes.Add(new NoteItem(note));
            }
            catch (HestiaApiException ex)
            {
                // TODO: Make actual error UI/UX to show this error
                Console.WriteLine(ex.Message);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;
        }
    }
}

