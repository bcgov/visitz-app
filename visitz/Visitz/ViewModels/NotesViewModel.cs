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

        [ObservableProperty]
        public string addNotesPlaceholder_NotePeriod = NoteItem.NotePeriodFrom(DateTime.Now);

        [ObservableProperty]
        public bool addNotesPlaceholder_ShowNotePeriod;

        [ObservableProperty]
        public string addNotesPlaceholder_ContentText;

        [ObservableProperty]
        public bool isAddNotesPlaceholderVisible;

        [ObservableProperty]
        public NoteItem latestNote;

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

            AddNotesPlaceholder_ShowNotePeriod = CaseIncident.EntityType == IcmEntity.Case;
            AddNotesPlaceholder_ContentText = CaseIncident.EntityType == IcmEntity.Case
                ? "There are no notes for this period"
                : $"There are no notes for this {CaseIncident.EntityType.ToLower()}";

            UpdateAddNotesPlaceholderVisibility();
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
            var canAppendNotes = !IsAddNotesPlaceholderVisible
                && Notes.First()?.NotePeriod == noteItem.NotePeriod
                && Notes.First()?.CreatedDate == noteItem.CreatedDate;
            await NoteDetailsPage.Open(VisitzPage, CaseIncident, noteItem, canAppendNotes);
        }

        [RelayCommand]
        public async void GoToNoteEntry()
        {
            await NoteEntryPage.Open(VisitzPage, CaseIncident, null);
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;
        }

        private void ApplyNotesQuery()
        {
            var notes = NotesQuery.AsEnumerable();

            ApplySorting(ref notes);

            if (notes.Any())
                LatestNote = notes.First();
            
            Notes = notes;
        }

        private void ApplySorting(ref IEnumerable<NoteItem> notes)
        {
            notes = notes.OrderByDescending(item => NoteItem.NotePeriodDateTimeTransform(item, false))
                .ThenByDescending(item => NoteItem.CreatedDateTimeTransform(item, false));
        }

        private void UpdateAddNotesPlaceholderVisibility()
        {
            bool showPlaceholder = ShouldShowAddNotesPlaceholder();

            (VisitzPage as NotesPage).ShowAddNotesPlaceholder(showPlaceholder);
            IsAddNotesPlaceholderVisible = showPlaceholder;
        }

        private bool ShouldShowAddNotesPlaceholder()
        {
            if (!Notes.Any())
                return true;
            else if (CaseIncident.EntityType == IcmEntity.Case)
                return !NoteItem.IsCurrentNotePeriod(Notes.First());
            else
                return false;
        }

        private void Notes_Changed(IRealmCollection<NoteItem> sender, ChangeSet changes)
        {
            if (changes == null) // Initial load
                return;

            ApplyNotesQuery();
            UpdateAddNotesPlaceholderVisibility();
        }
    }
}

