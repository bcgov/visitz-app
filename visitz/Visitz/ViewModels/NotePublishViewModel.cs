using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Resources.Localization;
using Visitz.Services;
using VisitzApi.Models;

namespace Visitz.ViewModels
{
    public partial class NotePublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
    {
        public CaseloadItem CaseloadItem { get; private set; }

        public NoteItem NoteItem { get; private set; }

        private SubmitNoteEntity submitNoteEntity;

        private string submitAndGetNotesServiceId;
        private string submitNotesServiceId;
        private string getNotesServiceId;

        public async void Init(CaseloadItem caseloadItem, NoteItem noteItem, string draft)
        {
            CaseloadItem = caseloadItem;
            NoteItem = noteItem;

            Title = noteItem?.PeriodOrPageNumber != null
                ? $"{caseloadItem.DisplayName} • {noteItem?.PeriodOrPageNumber}"
                : caseloadItem.DisplayName;

            var info = await VisitzSessionInfo.GetAsync();
            submitNoteEntity = new()
            {
                EntityNumber = caseloadItem.CaseIncidentNumber,
                EntityType = caseloadItem.EntityType,
                NotePeriod = noteItem?.NotePeriod ?? NoteItem.NotePeriodFrom(DateTime.Now),
                Content = NoteItem.WrapContent(info.Idir, DateTime.Now, draft),
                CreatedBy = info.Idir,
            };

            var id = CaseloadItem.CaseIncidentNumber;
            var notePeriod = submitNoteEntity.NotePeriod;

            submitAndGetNotesServiceId = SubmitAndGetNotesService.MakeId(id, notePeriod);
            submitNotesServiceId = SubmitNoteService.MakeId(id, notePeriod);
            getNotesServiceId = GetNotesService.MakeId(id);
        }

        public override void PageCreated()
        {
            base.PageCreated();

            Wait(LocalizedStrings.LoginToSubmitNotes);

            WeakReferenceMessenger.Default.Register(this, submitAndGetNotesServiceId);
            WeakReferenceMessenger.Default.Register(this, submitNotesServiceId);
            WeakReferenceMessenger.Default.Register(this, getNotesServiceId);

            Publish();
        }

        public override void PageDestroyed()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            base.PageDestroyed();
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
                else if (message.FinishedCancelled)
                    Cancel(LocalizedStrings.LoginToSubmitNotes);
            }
            else if (message.ServiceId == submitNotesServiceId)
            {
                if (message.FinishedError)
                    PublishError(LocalizedStrings.FailedToPublishToIcm, message.Message);
            }
        }
    }
}

