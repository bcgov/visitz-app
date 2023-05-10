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

        private readonly HestiApi hestiApi;

        public NotesViewModel(HestiApi hestiApi)
        {
            this.hestiApi = hestiApi;
        }

        public async void FetchNotes()
        {
            try
            {
                var notesList = await hestiApi.GetNotesAsync(CaseIncident.CaseIncidentNumber, CaseIncident.EntityType);

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

