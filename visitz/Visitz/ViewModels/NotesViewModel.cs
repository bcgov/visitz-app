using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    /// <summary>
    /// The business logic for the cases notes rendering goes here.
    /// </summary>
	public partial class NotesViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        public static readonly string CaseIncidentIdKey = "caseIncidentId";

        public string caseIncidentId;

        private Realm Realm { get; set; }

        [ObservableProperty]
        public CaseloadItem caseIncident;

        [ObservableProperty]
        public IEnumerable<NoteItem> notes;

        [ObservableProperty]
        public bool isRefreshing;

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            WeakReferenceMessenger.Default.Register(this, GetNotesService.MakeId(caseIncidentId));

            Realm = await VisitzRealm.GetIcmDataAsync();

            CaseIncident = Realm.Find<CaseloadItem>(caseIncidentId);

            Notes = Realm
                .All<NoteItem>()
                .Where(note => note.IcmId == caseIncidentId);
        }

        public override void PageDestroyed()
        {
            Notes = null;
            CaseIncident = null;

            Realm.Dispose();
            Realm = null;

            WeakReferenceMessenger.Default.Unregister<ServiceStateMessage, string>(this, 
                GetNotesService.MakeId(caseIncidentId));
        }

        [RelayCommand]
        public async Task CaseDetailsTapped()
        {
            await CaseloadItemDetailsPage.Open(VisitzPage, CaseIncident.CaseIncidentNumber);
        }

        [RelayCommand]
        public void RefreshNotes()
        {
            if (CaseIncident == null)
            {
                IsRefreshing = false;
                return;
            }
            var entityTuple = (CaseIncident.CaseIncidentNumber, CaseIncident.EntityType);
            WeakReferenceMessenger.Default.Send(GetNotesService.MakeStartMessage(entityTuple));
        }

        [RelayCommand]
        public async void GoToNoteDetails(NoteItem noteItem)
        {
            await NoteDetailsPage.Open(VisitzPage, noteItem); 
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;
        }
    }
}

