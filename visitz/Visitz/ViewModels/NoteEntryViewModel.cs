using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Models;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Storage;

namespace Visitz.ViewModels
{
    public partial class NoteEntryViewModel : VisitzViewModel, ICaseloadItemHolder
    {
        private static readonly int CharacterLimit = 16000;

        public static readonly string NoteItemKey = "noteItem";
        public static readonly string CaseIncidentKey = "caseIncident";

        public CaseloadItem CaseloadItem { get; set; }

        public NoteItem noteItem;

        [ObservableProperty]
        public string draft;

        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public string characterLimitText = $"{CharacterLimit}/{CharacterLimit}";

        private string noteDraftId;

        private IQueryable<NoteDraft> NoteDraftQuery { get; set; }

        private IDisposable NoteDraftQueryToken { get; set; }

        public override async void PageCreated()
        {
            base.PageCreated();

            CaseloadItem = Parameters[CaseIncidentKey] as CaseloadItem;
            noteItem = Parameters[NoteItemKey] as NoteItem;

            noteDraftId = NoteDraft.MakeId(CaseloadItem.CaseIncidentNumber);

            Title = noteItem?.PeriodOrPageNumber != null
                ? $"{CaseloadItem.DisplayName} • {noteItem?.PeriodOrPageNumber}"
                : CaseloadItem.DisplayName;

            var realm = await VisitzRealm.GetNoteDraftAsync();

            (NoteDraftQuery, NoteDraftQueryToken) = NoteDraft.Subscribe(realm, noteDraftId, NoteDraft_Changed);

            ApplyDraft();

            ClearDraftMessages();
        }

        public override void PageStopped()
        {
            SaveDraft();

            base.PageStopped();
        }

        public override void PageDestroyed()
        {
            NoteDraftQueryToken.Dispose();
            NoteDraftQueryToken = null;

            NoteDraftQuery = null;

            base.PageDestroyed();
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

            ShowDraftSavedMessage();
        }

        [RelayCommand]
		public async void PublishNotes()
		{
            var trimmedDraft = Draft?.Trim();

            if (trimmedDraft?.Length > 0)
                await NotePublishPage.Open(VisitzPage, CaseloadItem, noteItem, trimmedDraft);
        }

        public void EditorTextChanged(TextChangedEventArgs e)
        {
            if (string.Equals(e.OldTextValue, e.NewTextValue))
                // Early return required to prevent infinite loops due to "cancelling" events
                // by reassigning its previous value
                return;

            if (TextIsInvalid(e))
            {
                CancelTextChangedEvent(e);
                _ = (VisitzPage as NoteEntryPage).ShowEditorError(LocalizedStrings.InvalidEntry);
                return;
            }
            else if (ExceedsCharacterLimit(e))
            {
                CancelTextChangedEvent(e);
                _ = (VisitzPage as NoteEntryPage).ShowEditorError(LocalizedStrings.CharacterLimitReached);
                return;
            }

            UpdateCharLimit();
            ShowSavingDraftMessage();
        }

        private bool ExceedsCharacterLimit(TextChangedEventArgs e)
        {
            return e.NewTextValue?.Length > CharacterLimit;
        }

        private bool TextIsInvalid(TextChangedEventArgs e)
        {
            return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
        }

        private void CancelTextChangedEvent(TextChangedEventArgs e)
        {
            Draft = e.OldTextValue;
        }

        private void UpdateCharLimit()
        {
            CharacterLimitText = $"{CharacterLimit - (Draft?.Length ?? 0)}/{CharacterLimit}";
        }

        private void NoteDraft_Changed(IRealmCollection<NoteDraft> sender, ChangeSet changes)
        {
            if (changes == null) // Initial load
                return;

            ApplyDraft();
        }

        private async void ShowSavingDraftMessage()
        {
            await SetDraftMessageVisible(false, true);
        }

        private async void ShowDraftSavedMessage()
        {
            await SetDraftMessageVisible(true, false);
        }

        private async void ClearDraftMessages()
        {
            await SetDraftMessageVisible(false, false);
        }

        private async Task SetDraftMessageVisible(bool draftSaved, bool savingDraft)
        {
            if (VisitzPage is NoteEntryPage page)
            {
                await Task.WhenAll
                (
                    page.SetDraftSavedPromptVisible(draftSaved),
                    page.SetSavingDraftPromptVisible(savingDraft)
                );
            }
        }
    }
}
