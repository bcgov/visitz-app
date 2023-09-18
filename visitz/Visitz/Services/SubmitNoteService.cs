using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models;

namespace Visitz.Services
{
    public class SubmitNoteService : VisitzApiService
    {
        public static string MakeId(string entityNumber, string notePeriod)
        {
            return $"{nameof(SubmitNoteService)}-{entityNumber}-{notePeriod}";
        }

        public static StartServiceMessage MakeStartMessage(SubmitNoteEntity submitEntity)
        {
            return new StartServiceMessage()
            {
                Payload = submitEntity,
                ServiceId = MakeId(submitEntity.EntityNumber, submitEntity.NotePeriod),
                ServiceType = typeof(SubmitNoteService)
            };
        }

        private new SubmitNoteEntity Payload => (SubmitNoteEntity)base.Payload;

        public SubmitNoteService(Vpi vpi) : base(vpi) { }

        public override string GetId()
        {
            return MakeId(Payload.EntityNumber, Payload.NotePeriod);
        }

        protected override async Task RunAsync()
        {
            await SubmitNoteAsync();
        }

        private async Task SubmitNoteAsync()
        {
            var (status, _) = await Vpi.SubmitNotesAsync(Payload);

            ResultCode = status ? Result.Successful : Result.Error;
        }
    }
}
