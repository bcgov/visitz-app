using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models;

namespace Visitz.Services
{
    public class SubmitNoteService : VisitzApiService
    {
        public static string MakeId(SubmitNoteEntity submitEntity)
        {
            return $"{nameof(SubmitNoteService)}-{submitEntity.EntityNumber}-{submitEntity.NotePeriod}";
        }

        public static StartServiceMessage MakeStartMessage(SubmitNoteEntity submitEntity)
        {
            return new StartServiceMessage()
            {
                Payload = submitEntity,
                ServiceId = MakeId(submitEntity),
                ServiceType = typeof(SubmitNoteService)
            };
        }

        private new SubmitNoteEntity Payload => (SubmitNoteEntity)base.Payload;

        public SubmitNoteService(Vpi vpi) : base(vpi) { }

        public override string GetId()
        {
            return MakeId(Payload);
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
