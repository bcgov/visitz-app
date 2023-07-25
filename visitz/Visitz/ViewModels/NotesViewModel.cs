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

        [ObservableProperty]
        public CaseloadItem caseIncident;

        [ObservableProperty]
        public IEnumerable<NoteItem> notes;

        [ObservableProperty]
        public bool isRefreshing;

        private Realm Realm { get; set; }

        private IQueryable<NoteItem> NotesQuery { get; set; }

        private IDisposable NotesQueryToken { get; set; }

        public override async void PageCreated()
        {
            caseIncidentId = Parameters[CaseIncidentIdKey] as string;

            WeakReferenceMessenger.Default.Register(this, GetNotesService.MakeId(caseIncidentId));

            Realm = await VisitzRealm.GetIcmDataAsync();

            CaseIncident = Realm.Find<CaseloadItem>(caseIncidentId);

            NotesQuery = Realm.All<NoteItem>()
                .Where(note => note.IcmId == caseIncidentId);
            NotesQueryToken = NotesQuery.SubscribeForNotifications(Notes_Changed);

            ApplyNotesQuery();
        }

        public override void PageDestroyed()
        {
            Notes = null;
            CaseIncident = null;

            NotesQueryToken.Dispose();
            NotesQueryToken = null;

            Realm.Dispose();
            Realm = null;

            WeakReferenceMessenger.Default.UnregisterAll(this);
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
            await NoteDetailsPage.Open(VisitzPage, CaseIncident, noteItem);
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;
        }

        private void ApplyNotesQuery()
        {
            var notes = NotesQuery.AsEnumerable();

            ApplySorting(ref notes);

            Notes = notes;
        }

        private void ApplySorting(ref IEnumerable<NoteItem> notes)
        {
            notes = notes.OrderByDescending(NoteItem.NotePeriodDateTimeTransform)
                .ThenByDescending(NoteItem.CreatedDateTimeTransform);
        }

        private void Notes_Changed(IRealmCollection<NoteItem> sender, ChangeSet changes)
        {
            if (changes == null) // Initial load
                return;

            ApplyNotesQuery();
        }
    }
}

