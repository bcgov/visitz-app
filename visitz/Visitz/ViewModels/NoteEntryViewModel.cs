using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Events;
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
        private string DraftOutput => Draft?.Trim();

        [ObservableProperty]
        public string characterLimitText = $"{CharacterLimit}/{CharacterLimit}";

        [ObservableProperty]
        public bool allowPublish;

        [ObservableProperty]
        private NetworkAccess networkAccess = Connectivity.Current.NetworkAccess;

        private string noteDraftId;

        private IQueryable<NoteDraft> NoteDraftQuery { get; set; }

        private IDisposable NoteDraftQueryToken { get; set; }

        public event EventHandler<DraftErrorEventArgs> DraftError;

        public event EventHandler<DraftSaveStatusEventArgs> DraftSaveStateChanged;

        public override async void PageCreated()
        {
            base.PageCreated();

            CaseloadItem = Parameters[CaseIncidentKey] as CaseloadItem;
            noteItem = Parameters[NoteItemKey] as NoteItem;
            Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

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

            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

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
                await NotePublishPage.Open(VisitzPage, CaseloadItem, noteItem, trimmedDraft);
            if (UpdateAllowPublish())
            {
            }
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
                DraftError?.Invoke(this, new DraftErrorEventArgs(LocalizedStrings.InvalidEntry));
                return;
            }
            else if (ExceedsCharacterLimit(e))
            {
                CancelTextChangedEvent(e);
                DraftError?.Invoke(this, new DraftErrorEventArgs(LocalizedStrings.CharacterLimitReached));
                return;
            }

            UpdateAllowPublish(e.NewTextValue);
            UpdateCharLimit();
            ShowSavingDraftMessage();
        }

        private static bool ExceedsCharacterLimit(TextChangedEventArgs e)
        {
            return e.NewTextValue?.Length > CharacterLimit;
        }

        private static bool TextIsInvalid(TextChangedEventArgs e)
        {
            return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
        }

        private void CancelTextChangedEvent(TextChangedEventArgs e)
        {
            Draft = e.OldTextValue;
        }

        private void UpdateCharLimit()
        {
            CharacterLimitText = $"{CharacterLimit - (DraftOutput?.Length ?? 0)}/{CharacterLimit}";
        }

        private void NoteDraft_Changed(IRealmCollection<NoteDraft> sender, ChangeSet changes)
        {
            if (changes == null) // Initial load
                return;

            ApplyDraft();
        }

        private void ShowSavingDraftMessage()
        {
            SetDraftMessageVisible(false, true);
        }

        private void ShowDraftSavedMessage()
        {
            SetDraftMessageVisible(true, false);
        }

        private void ClearDraftMessages()
        {
            SetDraftMessageVisible(false, false);
        }

        private void SetDraftMessageVisible(bool draftSaved, bool savingDraft)
        {
            DraftSaveStateChanged?.Invoke(this, new DraftSaveStatusEventArgs(draftSaved, savingDraft));
        }

        partial void OnNetworkAccessChanged(NetworkAccess value)
        {
            UpdateAllowPublish();
        }

        private bool UpdateAllowPublish(string draftText = null)
        {
            draftText ??= DraftOutput;
            AllowPublish = NetworkAccess == NetworkAccess.Internet && draftText?.Length > 0;
            return AllowPublish;
        }

        private void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            NetworkAccess = e.NetworkAccess;
        }
    }
}
