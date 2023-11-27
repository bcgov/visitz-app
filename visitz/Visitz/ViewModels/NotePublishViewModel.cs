using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Storage;
using VisitzApi.Models;

namespace Visitz.ViewModels
{
    public partial class NotePublishViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
        public static readonly string DraftItemKey = "draft";
        public static readonly string NoteItemKey = "noteItem";
        public static readonly string CaseIncidentKey = "caseIncident";

        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public bool showPublishingIndicator = true;

        [ObservableProperty]
        public bool showRefreshIndicator = false;

        [ObservableProperty]
        public string publishingStatus = LocalizedStrings.PublishingNotesToIcm;

        [ObservableProperty]
        public string refreshingStatus = LocalizedStrings.RefreshingNotes;

        [ObservableProperty]
        public bool showRefreshStatusSection = false;

        [ObservableProperty]
        public bool showRetrySection = false;

        [ObservableProperty]
        public bool showRefreshSection = false;

        private bool isFetchOnly = false;
        private string draft;
        private SubmitNoteEntity submitNoteEntity;

        private bool wasDraftSubmitted = false;
        private bool wasNotesFetched = false;

        public override async void PageCreated()
        {
            base.PageCreated();

            var caseIncident = Parameters[CaseIncidentKey] as CaseloadItem;
            var noteItem = Parameters[NoteItemKey] as NoteItem;
            draft = Parameters[DraftItemKey] as string;

            Title = noteItem?.PeriodOrPageNumber != null
                ? $"{caseIncident.DisplayName} • {noteItem?.PeriodOrPageNumber}"
                : caseIncident.DisplayName;

            var notePeriod = (noteItem?.NotePeriod) ?? NoteItem.NotePeriodFrom(DateTime.Now);
            submitNoteEntity = new SubmitNoteEntity
            {
                EntityNumber = caseIncident.CaseIncidentNumber,
                EntityType = caseIncident.EntityType,
                NotePeriod = notePeriod,
            };
            WeakReferenceMessenger.Default.Register(this, SubmitAndGetNotesService.MakeId(caseIncident.CaseIncidentNumber, submitNoteEntity.NotePeriod));
            WeakReferenceMessenger.Default.Register(this, SubmitNoteService.MakeId(caseIncident.CaseIncidentNumber, submitNoteEntity.NotePeriod));
            WeakReferenceMessenger.Default.Register(this, GetNotesService.MakeId(caseIncident.CaseIncidentNumber));

            await PublishDraft(submitNoteEntity, draft);
        }

        public override void PageDestroyed()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            base.PageDestroyed();
        }

        private async Task PublishDraft(SubmitNoteEntity noteEntity, string draft)
        {
            var info = await VisitzSessionInfo.GetAsync();
            noteEntity.Content = NoteItem.WrapContent(info.Idir, DateTime.Now, draft);
            noteEntity.CreatedBy = info.Idir;

            ShowPublishingIndicator = true;
            ShowRefreshIndicator = false;
            PublishingStatus = LocalizedStrings.PublishingNotesToIcm;
            ShowRefreshSection = false;
            ShowRetrySection = false;

            WeakReferenceMessenger.Default.Send(SubmitAndGetNotesService.MakeStartMessage(noteEntity));
        }

        public async void Receive(ServiceStateMessage message)
        {
            if (message.Status != VisitzService.State.Stopped)
            {
                return;
            }

            if (message.ServiceId ==
                SubmitNoteService.MakeId(submitNoteEntity.EntityNumber, submitNoteEntity.NotePeriod))
            {
                wasDraftSubmitted = message.FinishedSuccess;

                ShowPublishingIndicator = false;
                if (wasDraftSubmitted)
                {
                    PublishingStatus = LocalizedStrings.NotesPublishedToIcm;
                    RefreshingStatus = LocalizedStrings.RefreshingNotes;
                    ShowRefreshIndicator = true;
                    ShowRefreshStatusSection = true;

                    using var realm = await VisitzRealm.GetNoteDraftAsync();
                    var noteDraft = realm.Find<NoteDraft>(NoteDraft.MakeId(submitNoteEntity.EntityNumber));

                    await realm.WriteAsync(() =>
                    {
                        if (noteDraft != null)
                        {
                            realm.Remove(noteDraft);
                            noteDraft = null;
                        }
                    });
                }
                else
                {
                    PublishingStatus = LocalizedStrings.FailedToPublishToIcm;
                }
            }
            else if (message.ServiceId ==
                GetNotesService.MakeId(submitNoteEntity.EntityNumber))
            {
                wasNotesFetched = message.FinishedSuccess;
                ShowRefreshIndicator = false;

                RefreshingStatus = wasNotesFetched
                    ? LocalizedStrings.RefreshedNotesOnDevice
                    : LocalizedStrings.FailedToRefreshNotes;

                if (isFetchOnly)
                {
                    await DismissOrAllowRetry(message);
                }
                isFetchOnly = false;
            }
            else if (message.ServiceId ==
                SubmitAndGetNotesService.MakeId(submitNoteEntity.EntityNumber, submitNoteEntity.NotePeriod))
            {
                await DismissOrAllowRetry(message);
            }
        }

        private async Task DismissOrAllowRetry(ServiceStateMessage message)
        {
            if (message.FinishedSuccess)
            {
                await Task.Delay(3000);
                await Dismiss();
                await Navigator.Navigation.PopAsync();
            }
            else
            {
                ShowRetrySection = true;
                if (!wasDraftSubmitted)
                {
                    PublishingStatus = LocalizedStrings.FailedToPublishToIcm;
                    ShowPublishingIndicator = false;
                }
            }
        }

        [RelayCommand]
        public async Task Dismiss()
        {
            await Navigator.Navigation.PopAsync();
        }

        [RelayCommand]
        public async void Retry()
        {
            ShowRetrySection = false;
            if (!wasDraftSubmitted)
            {
                await PublishDraft(submitNoteEntity, draft);
            }
            else
            {
                RefreshingStatus = LocalizedStrings.RefreshingNotes;
                ShowRefreshIndicator = true;
                ShowRefreshStatusSection = true;
                isFetchOnly = true;
                WeakReferenceMessenger.Default.Send(GetNotesService.MakeStartMessage(submitNoteEntity.EntityNumber, submitNoteEntity.EntityType));
            }
        }
    }
}

