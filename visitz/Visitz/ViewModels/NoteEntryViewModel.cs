using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Oidc;
using Realms;
using Visitz.Pages;
using Visitz.Resources.Localization;
using Visitz.Storage;
using VisitzApi.Models;
using VisitzModel;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Utilities;

namespace Visitz.ViewModels
{
    public partial class NoteEntryViewModel : VisitzViewModel, ICaseloadItemHolder
    {
        private static readonly int CharacterLimit = 16000;
		public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;

        public CaseloadItem CaseloadItem { get; set; }

        [ObservableProperty]
        public NoteDraft noteDraft;

        private string DraftOutput => NoteDraft?.Draft?.Trim();

        [ObservableProperty]
        public bool allowPublish;

		[ObservableProperty]
		public bool allowDiscard;

		[ObservableProperty]
		public int remainingCharacters = CharacterLimit;

        [ObservableProperty]
        private NetworkAccess networkAccess = Connectivity.Current.NetworkAccess;

        public event EventHandler<DraftErrorEventArgs> DraftError;

        public event EventHandler<DraftSaveStatusEventArgs> DraftSaveStateChanged;

		private readonly Debouncer debouncer = new(Debouncer.AvgStoppedTypingDelay);

		Realm Realm { get; set; }

        public override async void Create()
        {
            base.Create();

            Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

            await InitNoteDraft();
            ClearDraftMessages();
        }

        private async Task InitNoteDraft()
        {
            Realm = await VisitzRealms.GetNoteDraftsRealmAsync();
			NoteDraft = NoteDraft.FindByEntityId(Realm, CaseloadItem.CaseIncidentNumber) ?? CreateNoteDraft();
        }

		private NoteDraft CreateNoteDraft()
		{
			return new NoteDraft()
			{
				ParentEntityId = NoteDraft.MakeId(CaseloadItem.CaseIncidentNumber),
			};
		}

        public override void Destroy()
        {
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

			Realm.Dispose();

            base.Destroy();
        }

        [RelayCommand]
		public async Task PublishNotes()
		{
            if (UpdateAllowPublish())
            {
                await Navigator.Navigation.PopModalAsync();

                var notePublishVm = ServiceProvider.GetService<NotePublishViewModel>();

#pragma warning disable SS002 // DateTime.Now was referenced
				var now = DateTime.Now; // API system does not use UTC times
#pragma warning restore SS002 // DateTime.Now was referenced

				var info = await OidcSessionInfo.GetAsync();
				var submitNoteEntity = new SubmitNoteEntity
				{
					EntityNumber = CaseloadItem.CaseIncidentNumber,
					EntityType = CaseloadItem.EntityType,
					NotePeriod = NoteItem.NotePeriodFrom(now),
					Content = NoteItem.WrapContent(info.Idir, now, DraftOutput),
					CreatedBy = info.Idir,
				};

                notePublishVm.Init(CaseloadItem, submitNoteEntity);
                await Navigator.Navigation.PushAsync(new PublishPage(notePublishVm));
            }
        }

        public async Task EditorTextChanged(TextChangedEventArgs e)
        {
            if (string.Equals(e.OldTextValue, e.NewTextValue))
				// Early return required to prevent infinite loops due to "cancelling" events
				// by reassigning its previous value
				return;

			SetDraftInfo();

			int length = e.NewTextValue?.Length ?? 0;

			if (length > 0 && !NoteDraft.IsManaged)
				await Realm.WriteAsync(() => Realm.Add(NoteDraft));

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

			RemainingCharacters = CharacterLimit - length;
			AllowDiscard = NoteDraft.IsManaged;
            UpdateAllowPublish(e.NewTextValue);

			if (NoteDraft.IsManaged)
			{
				ShowSavingDraftMessage();
				await debouncer.Debounce(ShowDraftSavedMessage);
			}
			else
			{
				debouncer.Cancel();
				ClearDraftMessages();
			}
        }

        private static bool ExceedsCharacterLimit(TextChangedEventArgs e)
        {
            return e.NewTextValue?.Length > CharacterLimit;
        }

        private static bool TextIsInvalid(TextChangedEventArgs e)
        {
            return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
        }

		private void SetDraftInfo()
		{
			if (string.IsNullOrWhiteSpace(NoteDraft.DraftLocationBinding))
				NoteDraft.DraftLocationBinding = CaseloadItem.DisplayName;

			if (NoteDraft.RelatedEntityTypeBinding == EntityType.Unknown)
				NoteDraft.RelatedEntityTypeBinding = CaseloadItem.EntityType.ParseEntityType();

			if (NoteDraft.RelatedEntitySubtypeBinding == EntitySubtype.Unknown)
				NoteDraft.RelatedEntitySubtypeBinding = CaseloadItem.CaseIncidentType.ParseEntitySubtype();
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

		public async Task ResetDraftAsync()
		{
			if (!NoteDraft.IsManaged)
				return;

			await Realm.WriteAsync(() => Realm.Remove(NoteDraft));
			NoteDraft = CreateNoteDraft();
		}
	}
}
