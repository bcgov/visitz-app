using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Services;
using Visitz.Storage;
using VisitzApi.Models;

namespace Visitz.ViewModels
{
	public partial class NoteEntryViewModel : VisitzViewModel
    {
        public static readonly string NoteItemKey = "noteItem";
        public static readonly string CaseIncidentKey = "caseIncident";

        public CaseloadItem caseIncident;

        [ObservableProperty]
        public NoteItem noteItem;

        [ObservableProperty]
        public string draft;

        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public string characterLimitText = "16000/16000";

        private string noteDraftId;

        private IQueryable<NoteDraft> NoteDraftQuery { get; set; }

        private IDisposable NoteDraftQueryToken { get; set; }

        public override async void PageCreated()
        {
            caseIncident = Parameters[CaseIncidentKey] as CaseloadItem;
            NoteItem = Parameters[NoteItemKey] as NoteItem;

            noteDraftId = NoteDraft.MakeId(caseIncident.CaseIncidentNumber, NoteItem.CreatedDate);

            Title = $"{caseIncident.DisplayName} • {NoteItem.PeriodOrPageNumber}";

            var realm = await VisitzRealm.GetNoteDraftAsync();
            NoteDraftQuery = realm.All<NoteDraft>()
                .Where(draft => draft.CaseIncidentAndCreatedDateID == noteDraftId);

            NoteDraftQueryToken = NoteDraftQuery.SubscribeForNotifications(NoteDraft_Changed);

            ApplyDraft();
        }

        public override void PageStopped()
        {
            SaveDraft();
        }

        public override void PageDestroyed()
        {
            NoteDraftQueryToken.Dispose();
            NoteDraftQueryToken = null;

            NoteDraftQuery = null;
        }

        private void ApplyDraft()
        {
            Draft = NoteDraftQuery.FirstOrDefault()?.Draft;
            UpdateCharLimit();
        }

        [RelayCommand]
        public async void SaveDraft()
        {
            var realm = await VisitzRealm.GetNoteDraftAsync();
            var noteDraft = realm.Find<NoteDraft>(noteDraftId);

            await realm.WriteAsync(() =>
            {
                if (noteDraft == null)
                {
                    realm.Add(new NoteDraft
                    {
                        CaseIncidentAndCreatedDateID = noteDraftId,
                        Draft = Draft
                    });
                }
                else
                {
                    noteDraft.Draft = Draft;
                }
            });
        }

        [RelayCommand]
		public async void PublishNotes()
		{
            if (Draft?.Length > 0) {
                await NotePublishPage.OpenModal(VisitzPage, caseIncident, NoteItem, Draft);
            }
        }

        public void EditorTextChanged()
        {
            UpdateCharLimit();
        }

        private void UpdateCharLimit()
        {
            CharacterLimitText = $"{16000 - (Draft?.Length ?? 0)}/16000";
        }

        private void NoteDraft_Changed(IRealmCollection<NoteDraft> sender, ChangeSet changes)
        {
            if (changes == null) // Initial load
                return;

            ApplyDraft();
        }
    }
}

