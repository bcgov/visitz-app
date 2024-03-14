using CommunityToolkit.Mvvm.Messaging;
using Oidc;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Storage;
using VisitzApi.Models;
using VisitzModel.Models;

namespace Visitz.ViewModels
{
    public partial class NotePublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
    {
        private SubmitNoteEntity submitNoteEntity;

        private string submitAndGetNotesServiceId;
        private string submitNotesServiceId;
        private string getNotesServiceId;

        public void Init(CaseloadItem caseloadItem, SubmitNoteEntity submitNote)
        {
            Title = caseloadItem.DisplayName;
			submitNoteEntity = submitNote;

            var id = caseloadItem.CaseIncidentNumber;
            var notePeriod = submitNoteEntity.NotePeriod;

            submitAndGetNotesServiceId = SubmitAndGetNotesService.MakeId(id, notePeriod);
            submitNotesServiceId = SubmitNoteService.MakeId(id, notePeriod);
            getNotesServiceId = GetNotesService.MakeId(id);
        }

        public override void Create()
        {
            base.Create();

            Wait(LocalizedStrings.LoginToSubmitNotes);

            WeakReferenceMessenger.Default.Register(this, submitAndGetNotesServiceId);
            WeakReferenceMessenger.Default.Register(this, submitNotesServiceId);
            WeakReferenceMessenger.Default.Register(this, getNotesServiceId);

            Publish();
        }

        public override void Destroy()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            base.Destroy();
        }

        public override void Publish()
        {
            WeakReferenceMessenger.Default.Send(SubmitAndGetNotesService.MakeStartMessage(submitNoteEntity));
        }

        public async void Receive(ServiceStateMessage message)
        {
            if (message.ServiceId == submitAndGetNotesServiceId)
            {
                if (message.Status == VisitzService.State.Running)
                    Publishing(LocalizedStrings.PublishingNotesToIcm);
                else if (message.FinishedSuccess)
                    await Complete();
                else if (message.FinishedError)
                    PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
                else if (message.FinishedCancelled)
                    Cancel(LocalizedStrings.LoginToSubmitNotes);
            }
            else if (message.ServiceId == submitNotesServiceId)
            {
                if (message.FinishedSuccess)
                {
                    Published(LocalizedStrings.NotesPublishedToIcm);
                    await DiscardPublishedDraft();
                }
                if (message.FinishedError)
                    PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
            }
            else if (message.ServiceId == getNotesServiceId)
            {
                if (message.Status == VisitzService.State.Running)
                    Refreshing(LocalizedStrings.RefreshingNotes);
                else if (message.FinishedSuccess)
                    Refreshed(LocalizedStrings.RefreshedNotesOnDevice);
                else if (message.FinishedError)
                    RefreshError(LocalizedStrings.FailedToRefreshNotes, message.Message);
            }
        }

        private async Task DiscardPublishedDraft()
        {
            using var realm = await VisitzRealms.GetNoteDraftsRealmAsync();
            await NoteDraft.Delete(realm, submitNoteEntity.EntityNumber);
        }
    }
}

