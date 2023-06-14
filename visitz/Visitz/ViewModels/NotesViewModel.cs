using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Storage;
using Visitz.Views;
using VisitzApi;
using VisitzApi.ErrorHandling;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class NotesViewModel : VisitzViewModel
    {
        public static readonly string CaseIncidentIdKey = "caseIncidentId";

        private IQueryable<NoteItem> NotesQuery;

        public string caseIncidentId;

        [ObservableProperty]
        public CaseloadItem caseIncident;

        [ObservableProperty]
        public IEnumerable<NoteItem> notes;

        private Vpi Vpi { get; }

        public NotesViewModel(Vpi visitzApi)
        {
            Vpi = visitzApi;
        }

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            var realm = await IcmDataRealm.GetAsync();

            CaseIncident = realm.Find<CaseloadItem>(caseIncidentId);

            Notes = NotesQuery = realm
                .All<NoteItem>()
                .Where(note => note.IcmId == caseIncidentId);

            await TryFetchNotes();
        }

        private async Task TryFetchNotes()
        {
            if (await VisitzSession.GetValidSessionAsync())
                await FetchNotes();
        }

        private async Task FetchNotes()
        {
            try
            {
                var notesFromApi = await Vpi.GetNotesAsync(CaseIncident.CaseIncidentNumber, CaseIncident.EntityType);
                var notes = NoteItem.FromApiEntities(CaseIncident.CaseIncidentNumber, notesFromApi);

                using var realm = await IcmDataRealm.GetAsync();
                await realm.WriteAsync(() =>
                {
                    // NOTE 2023-06-05: The ICM API currently does not return PK info about notes.
                    // We can only associate them by the entity ID and the actual content of the
                    // note. So instead of using upserts, we're removing-then-adding new notes
                    // that come in.
                    realm.RemoveRange(NotesQuery);
                    realm.Add(notes);
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
        public async Task CaseDetailsTapped()
        {
            await CaseloadItemDetailsPage.Open(VisitzPage, CaseIncident.CaseIncidentNumber);
        }
    }
}

