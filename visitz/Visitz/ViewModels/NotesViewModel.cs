using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Visitz.Models;
using Visitz.Views;
using VisitzApi;
using VisitzApi.ErrorHandling;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class NotesViewModel : VisitzViewModel, IQueryAttributable
    {
        [ObservableProperty]
        public CaseloadItem caseIncident;

        public ObservableCollection<NoteItem> Notes { get; set; } = new();

        private Vpi Vpi { get; }

        public NotesViewModel(Vpi visitzApi)
        {
            Vpi = visitzApi;
        }

        public override void PageCreated()
        {
            FetchNotes();
        }

        public async void FetchNotes()
        {
            try
            {
                var notesList = await Vpi.GetNotesAsync(CaseIncident.CaseIncidentNumber, CaseIncident.EntityType);

                Notes.Clear();

                foreach (var note in notesList)
                    Notes.Add(new NoteItem(note));
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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;
        }

        public async Task CaseDetailsTapped()
        {
            await NavigateToCaseloadItemDetailsPage(CaseIncident);
        }

        private async Task NavigateToCaseloadItemDetailsPage(CaseloadItem caseloadItem)
        {
            await NavigateTo(typeof(CaseloadItemDetailsPage), new Dictionary<string, object> 
            { 
                { "caseIncident", caseloadItem }
            });
        }
    }
}

