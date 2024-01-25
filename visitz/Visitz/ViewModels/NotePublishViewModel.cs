using CommunityToolkit.Mvvm.Messaging;
using Visitz.Authentication.Keycloak;
using Visitz.Models;
using Visitz.Services;
using VisitzApi.Models;

namespace Visitz.ViewModels
{
    public partial class NotePublishViewModel : PublishViewModel, IRecipient<ServiceStateMessage>
    {
        public CaseloadItem CaseloadItem { get; private set; }

        public NoteItem NoteItem { get; private set; }

        private SubmitNoteEntity submitNoteEntity;

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
        }

        public override void Publish()
        {
            throw new NotImplementedException();
        }

        public void Receive(ServiceStateMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

