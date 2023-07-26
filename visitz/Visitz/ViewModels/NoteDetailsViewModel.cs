using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Storage;

namespace Visitz.ViewModels
{
	public partial class NoteDetailsViewModel : VisitzViewModel
    {
        public static readonly string NoteItemKey = "noteItem";
        public static readonly string CaseIncidentKey = "caseIncident";
        public static readonly string CanAppendNotesKey = "canAppendNotes";

        public CaseloadItem caseIncident;

        [ObservableProperty]
        public NoteItem noteItem;

        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public bool canAppendNotes;

        private Realm Realm { get; set; }

        private IQueryable<NoteItem> NoteItemQuery { get; set; }

        private IDisposable NotesQueryToken { get; set; }

        public override async void PageCreated()
        {
            caseIncident = Parameters[CaseIncidentKey] as CaseloadItem;
            NoteItem = Parameters[NoteItemKey] as NoteItem;
            CanAppendNotes = (bool)Parameters[CanAppendNotesKey];

            Title = $"{caseIncident.DisplayName} • {NoteItem.PeriodOrPageNumber}";

            var IcmId = NoteItem.IcmId;
            var CreatedDate = NoteItem.CreatedDate;
            var NotePeriod = NoteItem.NotePeriod;

            Realm = await VisitzRealm.GetIcmDataAsync();
            NoteItemQuery = Realm.All<NoteItem>()
                .Where(note => note.IcmId == IcmId
                && note.CreatedDate == CreatedDate
                && note.NotePeriod == NotePeriod);

            NotesQueryToken = NoteItemQuery.SubscribeForNotifications(Notes_Changed);
        }

        public override void PageDestroyed()
        {
            NotesQueryToken.Dispose();
            NotesQueryToken = null;

            NoteItemQuery = null;

            Realm.Dispose();
            Realm = null;
        }

        private void ApplyNotesQuery()
        {
            NoteItem = NoteItemQuery.FirstOrDefault();
        }

        [RelayCommand]
        public async void GoToNoteEntry(NoteItem noteItem)
        {
            await NoteEntryPage.Open(VisitzPage, caseIncident, noteItem);
        }

        private void Notes_Changed(IRealmCollection<NoteItem> sender, ChangeSet changes)
        {
            if (changes == null) // Initial load
                return;

            ApplyNotesQuery();
        }
    }
}

