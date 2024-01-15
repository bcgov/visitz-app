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
    public partial class NotePublishViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>, ICaseloadItemHolder
    {
        [ObservableProperty]
        public string title;

        public string Draft { get; set; }

        public string NotePeriod { get; set; }

        public CaseloadItem CaseloadItem { get; set; }

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
        private SubmitNoteEntity submitNoteEntity;

        private bool wasDraftSubmitted = false;
        private bool wasNotesFetched = false;

        public static string MakeTitle(CaseloadItem caseloadItem, NoteItem noteItem)
        {
            return noteItem?.PeriodOrPageNumber != null
                ? $"{caseloadItem.DisplayName} • {noteItem?.PeriodOrPageNumber}"
                : caseloadItem.DisplayName;
        }

        public void InitWith(CaseloadItem caseloadItem, NoteItem noteItem, string draft)
        {
            Title = MakeTitle(caseloadItem, noteItem);
            Draft = draft;
            CaseloadItem = caseloadItem;
            NotePeriod = (noteItem?.NotePeriod) ?? NoteItem.NotePeriodFrom(DateTime.Now);

            submitNoteEntity = new SubmitNoteEntity
            {
                EntityNumber = CaseloadItem.CaseIncidentNumber,
                EntityType = CaseloadItem.EntityType,
                NotePeriod = NotePeriod,
            };
        }

        public override async void PageCreated()
        {
            base.PageCreated();

            WeakReferenceMessenger.Default.Register(this, SubmitAndGetNotesService.MakeId(CaseloadItem.CaseIncidentNumber, submitNoteEntity.NotePeriod));
            WeakReferenceMessenger.Default.Register(this, SubmitNoteService.MakeId(CaseloadItem.CaseIncidentNumber, submitNoteEntity.NotePeriod));
            WeakReferenceMessenger.Default.Register(this, GetNotesService.MakeId(CaseloadItem.CaseIncidentNumber));

            await PublishDraft(submitNoteEntity, Draft);
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
                await PublishDraft(submitNoteEntity, Draft);
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

