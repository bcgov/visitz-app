using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Models;
using Visitz.Storage;
using Visitz.Pages;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Services;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class NotesViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        public static readonly string CaseIncidentIdKey = "caseIncidentId";

        public string caseIncidentId;

        [ObservableProperty]
        public CaseloadItem caseIncident;

        [ObservableProperty]
        public IEnumerable<NoteItem> notes;

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            WeakReferenceMessenger.Default.Register(this, GetNotesService.MakeId(caseIncidentId));

            var realm = await IcmDataRealm.GetAsync();

            CaseIncident = realm.Find<CaseloadItem>(caseIncidentId);

            Notes = realm
                .All<NoteItem>()
                .Where(note => note.IcmId == caseIncidentId);

            // TODO: Replace FetchNotes() with IsRefreshing implementation, like CaseloadViewModel
            FetchNotes();
        }

        private void FetchNotes()
        {
            var msg = GetNotesService.MakeStartMessage(caseIncidentId, CaseIncident.EntityType);
            WeakReferenceMessenger.Default.Send(msg);
        }

        [RelayCommand]
        public async Task CaseDetailsTapped()
        {
            await CaseloadItemDetailsPage.Open(VisitzPage, CaseIncident.CaseIncidentNumber);
        }

        public void Receive(ServiceStateMessage message)
        {
            // TODO: IsRefreshing implementation, like CaseloadViewModel
        }
    }
}

