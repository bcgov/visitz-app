using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Storage;
using VisitzModel;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Models;

namespace Visitz.ViewModels
{
    public partial class NoteEntryViewModel : VisitzViewModel, ICaseloadItemHolder
    {
        private static readonly int CharacterLimit = 16000;

        public CaseloadItem CaseloadItem { get; set; }

        [ObservableProperty]
        public NoteDraft noteDraft;

        private string DraftOutput => NoteDraft?.Draft?.Trim();

        [ObservableProperty]
        public bool allowPublish;

        [ObservableProperty]
        private NetworkAccess networkAccess = Connectivity.Current.NetworkAccess;

        public event EventHandler<DraftErrorEventArgs> DraftError;

        public event EventHandler<DraftSaveStatusEventArgs> DraftSaveStateChanged;

        public override async void PageCreated()
        {
            base.PageCreated();

            Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

            await InitNoteDraft();
            ClearDraftMessages();
        }

        private async Task InitNoteDraft()
        {
            var realm = await VisitzRealm.GetNoteDraftAsync();
            NoteDraft = NoteDraft.FindByEntityId(realm, CaseloadItem.CaseIncidentNumber);

            if (NoteDraft == null)
            {
                NoteDraft = new NoteDraft()
                {
                    CaseIncidentAndCreatedDateID = NoteDraft.MakeId(CaseloadItem.CaseIncidentNumber)
                };
                await realm.WriteAsync(() => realm.Add(NoteDraft));
            }
        }

        public override void PageDestroyed()
        {
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

            base.PageDestroyed();
        }

        [RelayCommand]
        public void UserStoppedTyping()
        {
            ConsoleTrace.TraceMethod(this);

            ShowDraftSavedMessage();
        }

        [RelayCommand]
		public async void PublishNotes()
		{
            if (UpdateAllowPublish())
            {
                await Navigator.Navigation.PopModalAsync();

                var notePublishVm = ServiceProvider.GetService<NotePublishViewModel>();
                var noteItem = NoteItem.GetLatestByEntityId(CaseloadItem.Realm, CaseloadItem.CaseIncidentNumber);
                
                await notePublishVm.Init(CaseloadItem, noteItem, DraftOutput);
                await Navigator.Navigation.PushAsync(new PublishPage(notePublishVm));
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
            NoteDraft.DraftBinding = e.OldTextValue;
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
