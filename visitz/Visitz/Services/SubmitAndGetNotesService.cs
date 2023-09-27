using Visitz.Authentication.Keycloak;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models;

namespace Visitz.Services
{
    public class SubmitAndGetNotesService : VisitzApiService
    {
        public static string MakeId(string entityNumber, string notePeriod)
        {
            return $"{nameof(SubmitAndGetNotesService)}-{entityNumber}-{notePeriod}";
        }

        public static StartServiceMessage MakeStartMessage(SubmitNoteEntity submitEntity)
        {
            return new StartServiceMessage()
            {
                ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.NotePeriod),
                ServiceType = typeof(SubmitAndGetNotesService),
                Payload = submitEntity,
            };
        }

        private new SubmitNoteEntity Payload => (SubmitNoteEntity)base.Payload;

        private ServiceHandler ServiceHandler { get; set; }

        public SubmitAndGetNotesService(Vpi vpi, ServiceHandler serviceHandler) : base(vpi)
        {
            ServiceHandler = serviceHandler;
        }

        public override string GetId()
        {
            return MakeId(Payload.EntityNumber, Payload.NotePeriod);
        }

        protected override async Task RunAsync()
        {
            await SubmitNote(Payload);
            await GetNotes(Payload.EntityNumber, Payload.EntityType);
            ResultCode = Result.Successful;
        }

        private async Task SubmitNote(SubmitNoteEntity noteEntity)
        {
            await ServiceHandler.TryRunServiceAsync(SubmitNoteService.MakeStartMessage(noteEntity));
        }

        private async Task GetNotes(string caseIncidentId, string entityType)
        {
            var startMessage = GetNotesService.MakeStartMessage(caseIncidentId, entityType);
            await ServiceHandler.TryRunServiceAsync(startMessage);
        }
    }
}

