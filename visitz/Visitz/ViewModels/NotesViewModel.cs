using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Visitz.Models.BOs;
using VisitzApi;
using VisitzApi.ErrorHandling;
using Visitz.Routers;

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

        NotesRouter Router { get; }

        private Vpi Vpi { get; }

        public NotesViewModel(NotesRouter router, Vpi visitzApi)
        {
            Router = router;
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
                // TODO: Make actual error UI/UX to show this error
                Console.WriteLine(ex.Message);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            CaseIncident = query["caseIncident"] as CaseloadItem;
        }

        public void CaseDetailsTapped()
        {
            Router.RouteUsing(CaseIncident);
        }
    }
}

